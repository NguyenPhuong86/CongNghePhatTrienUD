namespace AppDemo.Services;

public enum AiTextSource
{
    Provider,
    Fallback
}

public sealed record AiTextResult(
    string Text,
    AiTextSource Source,
    string? ErrorCategory = null);

public interface IAiTextService
{
    bool IsConfigured { get; }
    Task<AiTextResult> SuggestPresentationDescriptionAsync(
        string topic,
        string? speakerName,
        CancellationToken cancellationToken = default);
    Task<AiTextResult> SummarizePresentationsAsync(
        IEnumerable<string> presentations,
        CancellationToken cancellationToken = default);
}
