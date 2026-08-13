namespace AppDemo.ViewModels;

public class PresentationListViewModel
{
    public int PresentationId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Slides { get; set; }
    public string SpeakerName { get; set; } = string.Empty;
    public string SpeakerTitle { get; set; } = string.Empty;
}
