using AppDemo.Models;
using AppDemo.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Tests.Data;

public class EfCoreTests
{
    [Fact]
    public void SaveChanges_WhenOrganizerIsAdded_CanReadItBack()
    {
        using var scope = new SqliteContextScope();
        scope.Context.Organizers.Add(new Organizer { Name = "ICTU", Type = "Đại học", Experience = 20 });
        scope.Context.SaveChanges();
        Assert.Equal("ICTU", scope.Context.Organizers.Single().Name);
    }

    [Fact]
    public void Include_WhenFestivalHasOrganizer_LoadsRelationship()
    {
        using var scope = new SqliteContextScope();
        var organizer = new Organizer { Name = "ICTU", Type = "Đại học", Experience = 20 };
        scope.Context.Add(new Festival { Name = "Ngày hội", Location = "ICTU", Organizer = organizer });
        scope.Context.SaveChanges();
        scope.Context.ChangeTracker.Clear();

        var festival = scope.Context.Festivals.Include(item => item.Organizer).Single();
        Assert.Equal("ICTU", festival.Organizer!.Name);
    }
}
