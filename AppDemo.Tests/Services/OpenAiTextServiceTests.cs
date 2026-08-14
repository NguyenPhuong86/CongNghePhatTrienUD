using System.Net;
using System.Text;
using AppDemo.Options;
using AppDemo.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AppDemo.Tests.Services;

public sealed class OpenAiTextServiceTests
{
    [Fact]
    public async Task Suggest_ReturnsProviderResult_ForValidFixture()
    {
        var json = await File.ReadAllTextAsync(Fixture("responses-success.json"));
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Provider, result.Source);
        Assert.Equal("Nội dung từ nhà cung cấp.", result.Text);
        Assert.Null(result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsRateLimitFallback_For429()
    {
        var service = CreateService(HttpStatusCode.TooManyRequests, "{\"error\":{}}");

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("rate_limit", result.ErrorCategory);
        Assert.NotEmpty(result.Text);
    }

    [Fact]
    public async Task Suggest_ReturnsProviderResult_ForNestedOutputFixture()
    {
        const string json = """
            {"output":[{"content":[{"type":"output_text","text":"Nội dung lồng nhau."}]}]}
            """;
        var service = CreateService(HttpStatusCode.OK, json);

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Provider, result.Source);
        Assert.Equal("Nội dung lồng nhau.", result.Text);
    }

    [Fact]
    public async Task Suggest_ReturnsEmptyOutputFallback_WhenResponseHasNoText()
    {
        var service = CreateService(HttpStatusCode.OK, "{\"output\":[]}");

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("empty_output", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_ReturnsInvalidJsonFallback_ForMalformedResponse()
    {
        var service = CreateService(HttpStatusCode.OK, "not-json");

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("invalid_json", result.ErrorCategory);
    }

    [Fact]
    public async Task Suggest_DoesNotCallProvider_WhenApiKeyIsMissing()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "{}");
        var service = CreateService(handler, apiKey: null);

        var result = await service.SuggestPresentationDescriptionAsync("EF Core", "Lan");

        Assert.Equal(AiTextSource.Fallback, result.Source);
        Assert.Equal("not_configured", result.ErrorCategory);
        Assert.Equal(0, handler.CallCount);
    }

    private static OpenAiTextService CreateService(HttpStatusCode status, string body) =>
        CreateService(new StubHandler(status, body), "test-key");

    private static OpenAiTextService CreateService(StubHandler handler, string? apiKey)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };
        var options = Microsoft.Extensions.Options.Options.Create(new OpenAiOptions
        {
            ApiKey = apiKey,
            Model = "test-model",
            ResponsesEndpoint = "v1/responses"
        });
        return new OpenAiTextService(
            client,
            options,
            NullLogger<OpenAiTextService>.Instance);
    }

    private static string Fixture(string name) => Path.Combine(
        AppContext.BaseDirectory,
        "Fixtures",
        name);

    private sealed class StubHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            response.Headers.TryAddWithoutValidation("x-request-id", "req_test");
            return Task.FromResult(response);
        }
    }
}
