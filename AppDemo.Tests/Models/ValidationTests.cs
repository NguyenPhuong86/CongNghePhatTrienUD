using System.ComponentModel.DataAnnotations;
using AppDemo.Models;

namespace AppDemo.Tests.Models;

public class ValidationTests
{
    [Fact]
    public void Organizer_EmptyName_IsInvalid()
    {
        var model = new Organizer { Name = "", Type = "Trường đại học", Experience = 5 };
        var results = Validate(model);
        Assert.Contains(results, item => item.MemberNames.Contains(nameof(Organizer.Name)));
    }

    [Fact]
    public void Presentation_DurationBelowMinimum_IsInvalid()
    {
        var model = new Presentation { Topic = "MVC", Duration = 4, SpeakerId = 1 };
        var results = Validate(model);
        Assert.Contains(results, item => item.MemberNames.Contains(nameof(Presentation.Duration)));
    }

    [Fact]
    public void Festival_WithValidData_IsValid()
    {
        var model = new Festival { Name = "Ngày hội", Location = "Thái Nguyên", OrganizerId = 1 };
        Assert.Empty(Validate(model));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }
}
