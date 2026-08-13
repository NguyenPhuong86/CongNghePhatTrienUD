using AppDemo.Models;

namespace AppDemo.Tests.Models;

public class ErrorViewModelTests
{
    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
    {
        var model = new ErrorViewModel { RequestId = "request-123" };

        Assert.True(model.ShowRequestId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse(string? requestId)
    {
        var model = new ErrorViewModel { RequestId = requestId };

        Assert.False(model.ShowRequestId);
    }
}
