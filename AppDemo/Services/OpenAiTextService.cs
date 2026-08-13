using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AppDemo.Options;
using Microsoft.Extensions.Options;

namespace AppDemo.Services;

public class OpenAiTextService : IAiTextService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiTextService> _logger;
    private readonly OpenAiOptions _options;

    public OpenAiTextService(
        HttpClient httpClient,
        IOptions<OpenAiOptions> options,
        ILogger<OpenAiTextService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(GetApiKey());

    public async Task<AiTextResult> SuggestPresentationDescriptionAsync(
        string topic,
        string? speakerName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            return new AiTextResult(
                "Hãy nhập chủ đề bài trình bày để nhận gợi ý.",
                AiTextSource.Fallback,
                "validation");
        }

        var prompt = $"""
        Viết một mô tả ngắn bằng tiếng Việt cho bài trình bày hội thảo.
        Chủ đề: {topic}
        Diễn giả: {speakerName}
        Yêu cầu: 2 câu, rõ ràng, phù hợp ứng dụng quản lý hội thảo.
        """;

        return await GenerateAsync(prompt, () =>
            $"Bài trình bày \"{topic}\" giới thiệu các ý chính của chủ đề theo cách ngắn gọn, dễ tiếp cận. " +
            "Người tham dự có thể nắm được mục tiêu, lợi ích và ngữ cảnh ứng dụng sau phần trình bày.",
            cancellationToken);
    }

    public async Task<AiTextResult> SummarizePresentationsAsync(
        IEnumerable<string> presentations,
        CancellationToken cancellationToken = default)
    {
        var items = presentations
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Take(20)
            .ToList();

        if (items.Count == 0)
        {
            return new AiTextResult(
                "Chưa có bài trình bày nào để tóm tắt.",
                AiTextSource.Fallback,
                "empty_input");
        }

        var prompt = $"""
        Tóm tắt danh sách bài trình bày hội thảo bằng tiếng Việt.
        Viết 3 gạch đầu dòng ngắn gọn, nếu được hãy nêu chủ đề nổi bật.

        Danh sách:
        {string.Join("\n", items.Select((item, index) => $"{index + 1}. {item}"))}
        """;

        return await GenerateAsync(prompt, () =>
            "Danh sách bài trình bày tập trung vào các chủ đề chính của hội thảo. " +
            "Người quản trị có thể dùng tóm tắt này để viết phần giới thiệu chương trình.",
            cancellationToken);
    }

    private async Task<AiTextResult> GenerateAsync(
        string input,
        Func<string> fallback,
        CancellationToken cancellationToken)
    {
        var apiKey = GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Fallback(fallback, "not_configured");
        }

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                _options.ResponsesEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = _options.Model,
                instructions = "Bạn là trợ lý viết nội dung ngắn gọn cho ứng dụng quản lý hội thảo.",
                input,
                max_output_tokens = 300
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "OpenAI request failed. StatusCode={StatusCode}; RequestId={RequestId}; Category={Category}",
                    (int)response.StatusCode,
                    GetRequestId(response),
                    ClassifyStatus(response.StatusCode));

                return Fallback(fallback, ClassifyStatus(response.StatusCode));
            }

            var text = ExtractOutputText(responseText);
            return string.IsNullOrWhiteSpace(text)
                ? Fallback(fallback, "empty_output")
                : new AiTextResult(text, AiTextSource.Provider);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "OpenAI request timed out.");
            return Fallback(fallback, "timeout");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "OpenAI HTTP request failed.");
            return Fallback(fallback, "http_error");
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "OpenAI response contained invalid JSON.");
            return Fallback(fallback, "invalid_json");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI request failed.");
            return Fallback(fallback, "unexpected_error");
        }
    }

    private string? GetApiKey()
    {
        return _options.ApiKey
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    }

    private static AiTextResult Fallback(Func<string> factory, string category) =>
        new(factory(), AiTextSource.Fallback, category);

    private static string? GetRequestId(HttpResponseMessage response) =>
        response.Headers.TryGetValues("x-request-id", out var values)
            ? values.FirstOrDefault()
            : null;

    private static string ClassifyStatus(System.Net.HttpStatusCode statusCode) =>
        (int)statusCode switch
        {
            401 or 403 => "authentication",
            429 => "rate_limit",
            >= 500 => "provider_error",
            _ => "http_error"
        };

    private static string? ExtractOutputText(string json)
    {
        var root = JsonNode.Parse(json);
        var outputText = root?["output_text"]?.GetValue<string>();
        if (!string.IsNullOrWhiteSpace(outputText))
        {
            return outputText.Trim();
        }

        var output = root?["output"]?.AsArray();
        if (output == null)
        {
            return null;
        }

        var builder = new StringBuilder();
        foreach (var item in output)
        {
            var content = item?["content"]?.AsArray();
            if (content == null)
            {
                continue;
            }

            foreach (var part in content)
            {
                var text = part?["text"]?.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    builder.AppendLine(text);
                }
            }
        }

        var result = builder.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }
}
