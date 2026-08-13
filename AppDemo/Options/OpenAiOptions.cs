namespace AppDemo.Options;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gpt-5.4";
    public string BaseUrl { get; set; } = "https://api.openai.com/";
    public string ResponsesEndpoint { get; set; } = "v1/responses";
    public int TimeoutSeconds { get; set; } = 30;
}
