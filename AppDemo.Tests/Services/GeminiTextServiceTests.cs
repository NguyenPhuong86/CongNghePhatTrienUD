using System.Net;
using System.Text;
using System.Text.Json;
using AppDemo.Options;
using AppDemo.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AppDemo.Tests.Services;

public sealed class GeminiTextServiceTests
{
    [Fact]
    public async Task Suggest_ReturnsProviderResult_ForValidFixture()
    {
        var json = await File.ReadAllTextAsync(
            Fixture("gemini-generate-content-success.json"));
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Provider, result.Source);
        Assert.Equal("Nội dung từ Gemini.", result.Text);
        Assert.Null(result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_SendsGeminiEndpointHeaderAndPrompt()
    {
        const string json =
            "{\"candidates\":[{\"content\":{\"parts\":[{\"text\":\"OK\"}]}}]}";
        var handler = new StubHandler(HttpStatusCode.OK, json);
        var service = CreateService(handler, "test-key");

        await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(
            "/v1beta/models/test-model:generateContent",
            handler.RequestPath);
        Assert.Equal("test-key", handler.ApiKey);
        Assert.Contains("EF Core", handler.RequestBody);
        Assert.Contains("maxOutputTokens", handler.RequestBody);
    }

    [Fact]
    public async Task Suggest_ReturnsRateLimitFallback_For429()
    {
        var service = CreateService(
            HttpStatusCode.TooManyRequests,
            "{\"error\":{}}");

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("rate_limit", result.ErrorCategory);
        Assert.NotEmpty(result.Text);
    }

    [Fact]
    public async Task Suggest_JoinsMultipleTextParts()
    {
        const string json = """
            {"candidates":[{"content":{"parts":[
              {"text":"Phần một."},{"text":"Phần hai."}
            ]}}]}
            """;
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Provider, result.Source);
        Assert.Equal("Phần một.\nPhần hai.", result.Text.Replace("\r\n", "\n"));
    }

    [Fact]
    public async Task Suggest_ReturnsEmptyOutputFallback_WhenNoCandidateHasText()
    {
        var service = CreateService(
            HttpStatusCode.OK,
            "{\"candidates\":[]}");

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("empty_output", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsInvalidJsonFallback_ForMalformedResponse()
    {
        var service = CreateService(HttpStatusCode.OK, "not-json");

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("invalid_json", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsInvalidSchemaFallback_WhenCandidatesHasWrongType()
    {
        var service = CreateService(
            HttpStatusCode.OK,
            """{"candidates":{}}""");

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("invalid_schema", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_IgnoresPartsWithoutText()
    {
        const string json = """
            {"candidates":[{"content":{"parts":[{"thought":true}]}}]}
            """;
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("empty_output", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsFallback_WhenOutputExceedsFormLimit()
    {
        var json = JsonSerializer.Serialize(new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[] { new { text = new string('A', 1001) } }
                    }
                }
            }
        });
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("output_too_long", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsFallback_WhenUtf8ResponseExceedsByteLimit()
    {
        var json = JsonSerializer.Serialize(new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[] { new { text = new string('đ', 600_000) } }
                    }
                }
            }
        });
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("response_too_large", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_DoesNotCallProvider_WhenApiKeyIsMissing()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "{}");
        var service = CreateService(handler, apiKey: null);

        var result = await service
            .SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("not_configured", result.ErrorCategory);
        Assert.Equal(0, handler.CallCount);
    }

    private static GeminiTextService CreateService(
        HttpStatusCode status,
        string body) =>
        CreateService(new StubHandler(status, body), "test-key");

    private static GeminiTextService CreateService(
        StubHandler handler,
        string? apiKey)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };
        var options = Microsoft.Extensions.Options.Options.Create(
            new GeminiOptions
            {
                ApiKey = apiKey,
                Model = "test-model"
            });
        return new GeminiTextService(
            client,
            options,
            NullLogger<GeminiTextService>.Instance);
    }

    private static string Fixture(string name) => Path.Combine(
        AppContext.BaseDirectory,
        "Fixtures",
        name);

    private sealed class StubHandler(
        HttpStatusCode status,
        string body) : HttpMessageHandler
    {
        public int CallCount { get; private set; }
        public string? RequestPath { get; private set; }
        public string? ApiKey { get; private set; }
        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            RequestPath = request.RequestUri?.AbsolutePath;
            ApiKey = request.Headers.TryGetValues(
                "x-goog-api-key",
                out var values)
                ? values.Single()
                : null;
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            response.Headers.TryAddWithoutValidation(
                "x-goog-request-id",
                "req_test");
            return response;
        }
    }
}
