using AppDemo.Models;
using AppDemo.Services;
using System.ComponentModel.DataAnnotations;

namespace AppDemo.ViewModels;

public class AiAssistantViewModel
{
    [Required(ErrorMessage = "Chủ đề không được để trống")]
    [StringLength(150, ErrorMessage = "Chủ đề không được vượt quá 150 ký tự")]
    public string Topic { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Tên diễn giả không được vượt quá 100 ký tự")]
    public string? SpeakerName { get; set; }

    public string? Result { get; set; }
    public AiTextSource? ResultSource { get; set; }
    public string? ResultErrorCategory { get; set; }
    public string? Summary { get; set; }
    public AiTextSource? SummarySource { get; set; }
    public string? SummaryErrorCategory { get; set; }
    public bool IsAiConfigured { get; set; }
    public List<Presentation> Presentations { get; set; } = new();
}
