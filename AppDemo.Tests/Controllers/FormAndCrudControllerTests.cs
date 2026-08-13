using AppDemo.Controllers;
using AppDemo.Models;
using AppDemo.Tests.TestHelpers;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AppDemo.Tests.Controllers;

public class FormAndCrudControllerTests
{
    [Fact]
    public void Create_InvalidOrganizer_DoesNotSaveAndReturnsSameModel()
    {
        using var scope = new SqliteContextScope();
        var controller = new OrganizersController(scope.Context);
        var organizer = new Organizer();
        controller.ModelState.AddModelError(nameof(Organizer.Name), "required");

        var result = Assert.IsType<ViewResult>(controller.Create(organizer));
        Assert.Same(organizer, result.Model);
        Assert.Empty(scope.Context.Organizers);
    }

    [Fact]
    public void Create_InvalidFestival_RepopulatesOrganizerDropdown()
    {
        using var scope = new SqliteContextScope();
        scope.Context.Organizers.Add(new Organizer { Name = "ICTU", Type = "Đại học" });
        scope.Context.SaveChanges();
        var controller = new FestivalsController(scope.Context);
        controller.ModelState.AddModelError(nameof(Festival.Name), "required");

        var result = Assert.IsType<ViewResult>(controller.Create(new Festival()));
        Assert.NotNull(result.ViewData[nameof(scope.Context.Organizers)] ?? result.ViewData["Organizers"]);
        Assert.Empty(scope.Context.Festivals);
    }

    [Fact]
    public void Index_KeywordAndDescendingSort_ReturnsMatchingPresentations()
    {
        using var scope = new SqliteContextScope();
        var speaker = new Speaker { Name = "An", Title = "Kỹ sư" };
        scope.Context.Presentations.AddRange(
            new Presentation { Topic = "ASP.NET Core", Duration = 60, Speaker = speaker },
            new Presentation { Topic = "Cơ sở dữ liệu", Duration = 30, Speaker = speaker });
        scope.Context.SaveChanges();

        var controller = new PresentationsController(scope.Context);
        var result = Assert.IsType<ViewResult>(controller.Index("ASP", "duration_desc", 1));
        var model = Assert.IsType<PagedResult<PresentationListViewModel>>(result.Model);
        Assert.Single(model.Items);
        Assert.Equal("ASP.NET Core", model.Items[0].Topic);
    }

    [Fact]
    public void Index_PageBeyondRange_ClampsToLastPage()
    {
        using var scope = new SqliteContextScope();
        var controller = new SpeakersController(scope.Context);
        var result = Assert.IsType<ViewResult>(controller.Index(null, "name_asc", 99));
        var model = Assert.IsType<PagedResult<Speaker>>(result.Model);
        Assert.Equal(1, model.Page);
    }

    [Fact]
    public async Task Edit_WhenRouteIdDiffers_ReturnsBadRequest()
    {
        using var scope = new SqliteContextScope();
        var controller = new PresentationsController(scope.Context);
        var model = new PresentationEditViewModel { PresentationId = 2 };
        Assert.IsType<BadRequestResult>(await controller.Edit(1, model, CancellationToken.None));
    }

    [Fact]
    public void DeleteConfirmed_WhenIdDoesNotExist_ReturnsNotFound()
    {
        using var scope = new SqliteContextScope();
        var controller = new SpeakersController(scope.Context);
        Assert.IsType<NotFoundResult>(controller.DeleteConfirmed(404));
    }
}
