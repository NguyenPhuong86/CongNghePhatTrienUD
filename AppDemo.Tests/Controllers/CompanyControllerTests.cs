using AppDemo.Controllers;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Tests.Controllers;

public class CompanyControllerTests
{
    private readonly CompanyController _controller = new();

    [Fact]
    public void Index_WhenCalled_ReturnsCompanyIndexViewModel()
    {
        var result = Assert.IsType<ViewResult>(_controller.Index());
        var model = Assert.IsType<CompanyIndexViewModel>(result.Model);
        Assert.Equal("Công ty ABC", model.Company.Name);
    }

    [Fact]
    public void Index_WhenCalled_ReturnsTwoFeaturedServices()
    {
        var result = Assert.IsType<ViewResult>(_controller.Index());
        var model = Assert.IsType<CompanyIndexViewModel>(result.Model);
        Assert.Equal(2, model.FeaturedServices.Count);
    }

    [Fact]
    public void Contact_WhenCalled_ReturnsCompanyContact()
    {
        var result = Assert.IsType<ViewResult>(_controller.Contact());
        var model = Assert.IsType<CompanyContact>(result.Model);
        Assert.Contains("@", model.Email);
    }

    [Fact]
    public void Services_WhenCalled_ReturnsThreeServices()
    {
        var result = Assert.IsType<ViewResult>(_controller.Services());
        var model = Assert.IsType<List<CompanyService>>(result.Model);
        Assert.Equal(3, model.Count);
    }
}
