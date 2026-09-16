using System.ComponentModel.DataAnnotations;

namespace AppDemo.Options;

public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string? ApiKey { get; set; }

    [Required]
    public string Model { get; set; } = "gemini-3.5-flash";

    [Range(1, 120)]
    public int TimeoutSeconds { get; set; } = 30;
}
