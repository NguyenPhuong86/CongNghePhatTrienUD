using System.ComponentModel.DataAnnotations;

namespace AppDemo.Models;

public class Festival
{
    public int FestivalId { get; set; }

    [Required(ErrorMessage = "Tên lễ hội không được để trống")]
    [StringLength(120, ErrorMessage = "Tên lễ hội không được vượt quá 120 ký tự")]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Địa điểm không được để trống")]
    [StringLength(150, ErrorMessage = "Địa điểm không được vượt quá 150 ký tự")]
    public string Location { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Phải chọn đơn vị tổ chức")]
    public int OrganizerId { get; set; }

    public Organizer? Organizer { get; set; }
}
