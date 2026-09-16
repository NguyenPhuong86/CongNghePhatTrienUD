using System.Text;
using System.Text.Json;
using AppDemo.Options;
using Microsoft.Extensions.Options;

namespace AppDemo.Services;

public class GeminiTextService : IAiTextService
{
    private const int MaxResponseBytes = 1_000_000;
    private const int MaxResponseCharacters = 1_000_000;
    private const int MaxSuggestionCharacters = 1_000;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiTextService> _logger;
    private readonly GeminiOptions _options;

    public GeminiTextService(
        HttpClient httpClient,
        IOptions<GeminiOptions> options,
        ILogger<GeminiTextService> logger)
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

        var fallbackText =
            $"Bài trình bày \"{topic}\" giới thiệu các ý chính " +
            "của chủ đề theo cách ngắn gọn, dễ tiếp cận. " +
            "Người tham dự có thể nắm được mục tiêu, lợi ích " +
            "và ngữ cảnh ứng dụng sau phần trình bày.";

        return await GenerateAsync(prompt, () => fallbackText,
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
            var model = Uri.EscapeDataString(_options.Model);
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"v1beta/models/{model}:generateContent");
            request.Headers.TryAddWithoutValidation("x-goog-api-key", apiKey);

            const string instructions =
                "Bạn là trợ lý viết nội dung ngắn gọn " +
                "cho ứng dụng quản lý hội thảo.";

            var body = new
            {
                systemInstruction = new
                {
                    parts = new[] { new { text = instructions } }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = input } }
                    }
                },
                generationConfig = new { maxOutputTokens = 300 }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.Content.Headers.ContentLength > MaxResponseBytes)
            {
                return Fallback(fallback, "response_too_large");
            }

            var responseText = await response.Content
                .ReadAsStringAsync(cancellationToken);
            if (responseText.Length > MaxResponseCharacters)
            {
                return Fallback(fallback, "response_too_large");
            }

            if (!response.IsSuccessStatusCode)
            {
                var category = ClassifyStatus(response.StatusCode);
                _logger.LogWarning(
                    "Gemini request failed. StatusCode={StatusCode}; " +
                    "RequestId={RequestId}; Category={Category}",
                    (int)response.StatusCode,
                    GetRequestId(response),
                    category);

                return Fallback(fallback, category);
            }

            var text = ExtractOutputText(responseText);
            if (string.IsNullOrWhiteSpace(text))
            {
                return Fallback(fallback, "empty_output");
            }

            return text.Length > MaxSuggestionCharacters
                ? Fallback(fallback, "output_too_long")
                : new AiTextResult(text, AiTextSource.Provider);
        }
        catch (OperationCanceledException ex)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Gemini request timed out.");
            return Fallback(fallback, "timeout");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Gemini HTTP request failed.");
            return Fallback(fallback, "http_error");
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Gemini response contained invalid JSON.");
            return Fallback(fallback, "invalid_json");
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Gemini response had an invalid schema.");
            return Fallback(fallback, "invalid_schema");
        }
    }

    private string? GetApiKey()
    {
        return _options.ApiKey
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
    }

    private static AiTextResult Fallback(Func<string> factory, string category) =>
        new(factory(), AiTextSource.Fallback, category);

    private static string? GetRequestId(HttpResponseMessage response)
    {
        foreach (var header in new[] { "x-request-id", "x-goog-request-id" })
        {
            if (response.Headers.TryGetValues(header, out var values))
            {
                return values.FirstOrDefault();
            }
        }

        return null;
    }

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
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Response root must be an object.");
        }

        if (!root.TryGetProperty("candidates", out var candidates))
        {
            throw new InvalidDataException("Response has no candidates.");
        }
        if (candidates.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("candidates must be an array.");
        }

        var builder = new StringBuilder();
        foreach (var candidate in candidates.EnumerateArray().Take(5))
        {
            if (!candidate.TryGetProperty("content", out var content) ||
                content.ValueKind != JsonValueKind.Object ||
                !content.TryGetProperty("parts", out var parts) ||
                parts.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var part in parts.EnumerateArray().Take(20))
            {
                if (!part.TryGetProperty("text", out var text))
                {
                    continue;
                }
                if (text.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidDataException(
                        "Candidate text must be a string.");
                }

                var value = text.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    builder.AppendLine(value);
                }
            }
        }

        var result = builder.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }
}
