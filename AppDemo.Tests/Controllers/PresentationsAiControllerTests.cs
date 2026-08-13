using AppDemo.Controllers;
using AppDemo.Models;
using AppDemo.Services;
using AppDemo.Tests.TestHelpers;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AppDemo.Tests.Controllers;

public class PresentationsAiControllerTests
{
    [Fact]
    public async Task SuggestDescription_WhenTopicIsEmpty_DoesNotCallAiOrSave()
    {
        using var scope = new SqliteContextScope();
        var ai = new Mock<IAiTextService>();
        var controller = new PresentationsController(scope.Context, ai.Object);
        var model = new PresentationEditViewModel();

        var result = Assert.IsType<ViewResult>(
            await controller.SuggestDescription(model, CancellationToken.None));

        Assert.Equal("Create", result.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(scope.Context.Presentations);
        ai.Verify(
            service => service.SuggestPresentationDescriptionAsync(
                It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SuggestDescription_WhenSpeakerDoesNotExist_AddsModelError()
    {
        using var scope = new SqliteContextScope();
        var ai = new Mock<IAiTextService>();
        var controller = new PresentationsController(scope.Context, ai.Object);
        var model = new PresentationEditViewModel { Topic = "ASP.NET Core", SpeakerId = 404 };

        var result = Assert.IsType<ViewResult>(
            await controller.SuggestDescription(model, CancellationToken.None));

        Assert.Equal("Create", result.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(scope.Context.Presentations);
    }

    [Fact]
    public async Task SuggestDescription_WhenValid_ReturnsDraftWithoutSaving()
    {
        using var scope = new SqliteContextScope();
        var speaker = new Speaker { Name = "Nguyễn An", Title = "Kỹ sư" };
        scope.Context.Speakers.Add(speaker);
        await scope.Context.SaveChangesAsync();
        var ai = new Mock<IAiTextService>();
        ai.Setup(service => service.SuggestPresentationDescriptionAsync(
                "ASP.NET Core", "Nguyễn An", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AiTextResult(
                "Bản nháp do AI tạo.",
                AiTextSource.Provider));
        var controller = new PresentationsController(scope.Context, ai.Object);
        var model = new PresentationEditViewModel
        {
            Topic = "ASP.NET Core",
            Duration = 60,
            SpeakerId = speaker.SpeakerId
        };

        var result = Assert.IsType<ViewResult>(
            await controller.SuggestDescription(model, CancellationToken.None));
        var returnedModel = Assert.IsType<PresentationEditViewModel>(result.Model);

        Assert.Equal("Bản nháp do AI tạo.", returnedModel.Description);
        Assert.True(returnedModel.WasGeneratedByAi);
        Assert.Empty(scope.Context.Presentations);
        ai.Verify(service => service.SuggestPresentationDescriptionAsync(
            "ASP.NET Core", "Nguyễn An", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenValid_SavesConfirmedDescription()
    {
        using var scope = new SqliteContextScope();
        var speaker = new Speaker { Name = "Nguyễn An", Title = "Kỹ sư" };
        scope.Context.Speakers.Add(speaker);
        await scope.Context.SaveChangesAsync();
        var controller = new PresentationsController(scope.Context, Mock.Of<IAiTextService>());
        var model = new PresentationEditViewModel
        {
            Topic = "ASP.NET Core",
            Duration = 60,
            SpeakerId = speaker.SpeakerId,
            Description = "Nội dung đã được người quản trị xác nhận."
        };

        Assert.IsType<RedirectToActionResult>(
            await controller.Create(model, CancellationToken.None));

        var saved = Assert.Single(scope.Context.Presentations);
        Assert.Equal(model.Description, saved.Description);
    }
}
