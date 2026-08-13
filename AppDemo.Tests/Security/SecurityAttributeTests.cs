using System.Reflection;
using AppDemo.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Tests.Security;

public class SecurityAttributeTests
{
    [Fact]
    public void CreateGet_WhenInspected_RequiresAdminRole()
    {
        var method = typeof(SpeakersController).GetMethod(nameof(SpeakersController.Create), Type.EmptyTypes)!;
        var attribute = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.Equal("Admin", attribute?.Roles);
    }

    [Fact]
    public void CreatePost_WhenInspected_UsesAntiforgeryValidation()
    {
        var method = typeof(SpeakersController).GetMethod(nameof(SpeakersController.Create), [typeof(AppDemo.Models.Speaker)])!;
        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
    }
}
