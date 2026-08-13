using System.ComponentModel.DataAnnotations;

namespace AppDemo.Models;

public class Speaker
{
    public int SpeakerId { get; set; }

    [Required(ErrorMessage = "Tên diễn giả không được để trống")]
    [StringLength(100, ErrorMessage = "Tên diễn giả không được vượt quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Chức danh không được để trống")]
    [StringLength(100, ErrorMessage = "Chức danh không được vượt quá 100 ký tự")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Tiểu sử không được vượt quá 500 ký tự")]
    public string? Bio { get; set; }

    public ICollection<Presentation> Presentations { get; set; } = new List<Presentation>();
}
