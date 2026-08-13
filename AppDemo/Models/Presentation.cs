using System.ComponentModel.DataAnnotations;

namespace AppDemo.Models;

public class Presentation
{
    public int PresentationId { get; set; }

    [Required(ErrorMessage = "Chủ đề không được để trống")]
    [StringLength(150, ErrorMessage = "Chủ đề không được vượt quá 150 ký tự")]
    public string Topic { get; set; } = string.Empty;

    [Range(5, 240, ErrorMessage = "Thời lượng phải từ 5 đến 240 phút")]
    public int Duration { get; set; }

    [StringLength(250, ErrorMessage = "Đường dẫn slide không được vượt quá 250 ký tự")]
    public string? Slides { get; set; }

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Phải chọn diễn giả")]
    public int SpeakerId { get; set; }

    public Speaker? Speaker { get; set; }
}
