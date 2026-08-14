using AppDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppDemo.Tests.Controllers;

public sealed class HomeControllerTests
{
    [Fact]
    public void About_WhenRequested_ReturnsView()
    {
        var controller = new HomeController();

        var result = controller.About();

        Assert.IsType<ViewResult>(result);
    }
}
