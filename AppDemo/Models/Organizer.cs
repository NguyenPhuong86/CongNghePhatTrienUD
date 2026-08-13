using System.ComponentModel.DataAnnotations;

namespace AppDemo.Models;

public class Organizer
{
    public int OrganizerId { get; set; }

    [Required(ErrorMessage = "Tên đơn vị không được để trống")]
    [StringLength(100, ErrorMessage = "Tên đơn vị không được vượt quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại đơn vị không được để trống")]
    [StringLength(50, ErrorMessage = "Loại đơn vị không được vượt quá 50 ký tự")]
    public string Type { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 100")]
    public int Experience { get; set; }

    public ICollection<Festival> Festivals { get; set; } = new List<Festival>();
}
