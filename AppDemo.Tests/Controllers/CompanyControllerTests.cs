using AppDemo.Controllers;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Tests.Controllers;

public class CompanyControllerTests
{
    [Fact]
    public void Index_WhenCalled_ReturnsCompanyIndexViewModel()
    {
        var result = new CompanyController().Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CompanyIndexViewModel>(viewResult.Model);
        Assert.NotNull(model.Company);
        Assert.Equal(2, model.FeaturedServices.Count);
    }

    [Fact]
    public void Contact_WhenCalled_ReturnsCompanyContact()
    {
        var result = new CompanyController().Contact();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CompanyContact>(viewResult.Model);
        Assert.Contains("@", model.Email);
        Assert.False(string.IsNullOrWhiteSpace(model.Phone));
    }

    [Fact]
    public void Services_WhenCalled_ReturnsThreeServices()
    {
        var result = new CompanyController().Services();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<CompanyService>>(viewResult.Model);
        Assert.Equal(3, model.Count);
        Assert.All(model, service => Assert.True(service.EstimatedDays > 0));
    }
}
