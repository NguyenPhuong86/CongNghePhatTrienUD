using AppDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace AppDemo.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.Migrate();
        NormalizeLegacyDemoData(context);

        var culturalCenter = FindOrganizer(
            context,
            "Trung tâm Văn hóa Thành phố");
        culturalCenter.Name = "Trung tâm Văn hóa Thành phố";
        culturalCenter.Type = "Đơn vị sự nghiệp";
        culturalCenter.Experience = 12;

        var eventCompany = FindOrganizer(
            context,
            "Công ty Sự kiện Mùa Xuân");
        eventCompany.Name = "Công ty Sự kiện Mùa Xuân";
        eventCompany.Type = "Doanh nghiệp";
        eventCompany.Experience = 7;

        context.SaveChanges();

        var foodFestival = FindFestival(
            context,
            "Lễ hội Ẩm thực Địa phương");
        foodFestival.Name = "Lễ hội Ẩm thực Địa phương";
        foodFestival.Date = new DateTime(2026, 6, 12);
        foodFestival.Location = "Quảng trường trung tâm";
        foodFestival.OrganizerId = culturalCenter.OrganizerId;

        var summerFestival = FindFestival(
            context,
            "Ngày hội Văn hóa Mùa hè");
        summerFestival.Name = "Ngày hội Văn hóa Mùa hè";
        summerFestival.Date = new DateTime(2026, 7, 20);
        summerFestival.Location = "Công viên Sông Cầu";
        summerFestival.OrganizerId = eventCompany.OrganizerId;

        context.SaveChanges();

        var softwareArchitect = FindSpeaker(
            context,
            "Nguyễn Minh Anh");
        softwareArchitect.Name = "Nguyễn Minh Anh";
        softwareArchitect.Title = "Kiến trúc sư phần mềm";
        softwareArchitect.Bio = "Chuyên gia thiết kế hệ thống web với ASP.NET Core.";

        var dataEngineer = FindSpeaker(
            context,
            "Trần Bảo Châu");
        dataEngineer.Name = "Trần Bảo Châu";
        dataEngineer.Title = "Kỹ sư dữ liệu";
        dataEngineer.Bio = "Phụ trách các giải pháp dữ liệu và báo cáo thông minh.";

        context.SaveChanges();

        var mvcPresentation = FindPresentation(
            context,
            "Xây dựng ứng dụng MVC với EF Core");
        mvcPresentation.Topic = "Xây dựng ứng dụng MVC với EF Core";
        mvcPresentation.Duration = 60;
        mvcPresentation.Slides = "slides/mvc-ef-core.pdf";
        mvcPresentation.SpeakerId = softwareArchitect.SpeakerId;

        var linqPresentation = FindPresentation(
            context,
            "Truy vấn LINQ hiệu quả");
        linqPresentation.Topic = "Truy vấn LINQ hiệu quả";
        linqPresentation.Duration = 45;
        linqPresentation.Slides = "slides/linq-query.pdf";
        linqPresentation.SpeakerId = dataEngineer.SpeakerId;

        var crudPresentation = FindPresentation(
            context,
            "Kiểm thử CRUD trong ứng dụng web");
        crudPresentation.Topic = "Kiểm thử CRUD trong ứng dụng web";
        crudPresentation.Duration = 50;
        crudPresentation.Slides = "slides/crud-testing.pdf";
        crudPresentation.SpeakerId = softwareArchitect.SpeakerId;

        context.SaveChanges();
    }

    private static void NormalizeLegacyDemoData(ApplicationDbContext context)
    {
        var legacySpeaker = context.Speakers
            .AsEnumerable()
            .FirstOrDefault(s => MatchesIgnoringDiacritics(s.Name, "Lê Thị Kiểm Thử"));
        if (legacySpeaker != null)
        {
            legacySpeaker.Name = "Lê Thị Kiểm Thử";
            legacySpeaker.Title = "Giảng viên";
            legacySpeaker.Bio = "Kiểm tra tạo diễn giả từ form.";
        }

        var legacyPresentation = context.Presentations
            .AsEnumerable()
            .FirstOrDefault(p =>
                p.Topic.Contains("Demo CRUD", StringComparison.OrdinalIgnoreCase)
                && p.Topic.Contains("LINQ", StringComparison.OrdinalIgnoreCase)
                && p.Topic.Contains("EF Core", StringComparison.OrdinalIgnoreCase));
        if (legacyPresentation != null)
        {
            legacyPresentation.Topic = "Demo CRUD với LINQ và EF Core";
            legacyPresentation.Duration = Math.Max(legacyPresentation.Duration, 30);
        }

        context.SaveChanges();
    }

    private static Organizer FindOrganizer(
        ApplicationDbContext context,
        string name)
    {
        var organizer = context.Organizers
            .AsEnumerable()
            .FirstOrDefault(o => MatchesIgnoringDiacritics(o.Name, name));

        if (organizer != null)
        {
            return organizer;
        }

        organizer = new Organizer();
        context.Organizers.Add(organizer);
        return organizer;
    }

    private static Festival FindFestival(
        ApplicationDbContext context,
        string name)
    {
        var festival = context.Festivals
            .AsEnumerable()
            .FirstOrDefault(f => MatchesIgnoringDiacritics(f.Name, name));

        if (festival != null)
        {
            return festival;
        }

        festival = new Festival();
        context.Festivals.Add(festival);
        return festival;
    }

    private static Speaker FindSpeaker(
        ApplicationDbContext context,
        string name)
    {
        var speaker = context.Speakers
            .AsEnumerable()
            .FirstOrDefault(s => MatchesIgnoringDiacritics(s.Name, name));

        if (speaker != null)
        {
            return speaker;
        }

        speaker = new Speaker();
        context.Speakers.Add(speaker);
        return speaker;
    }

    private static Presentation FindPresentation(
        ApplicationDbContext context,
        string topic)
    {
        var presentation = context.Presentations
            .AsEnumerable()
            .FirstOrDefault(p => MatchesIgnoringDiacritics(p.Topic, topic));

        if (presentation != null)
        {
            return presentation;
        }

        presentation = new Presentation();
        context.Presentations.Add(presentation);
        return presentation;
    }

    private static bool MatchesIgnoringDiacritics(string value, string expected)
    {
        return string.Equals(
            RemoveDiacritics(value),
            RemoveDiacritics(expected),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveDiacritics(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
