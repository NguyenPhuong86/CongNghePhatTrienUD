using AppDemo.Data;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Controllers;

public class FestivalsController : Controller
{
    private readonly ApplicationDbContext _context;

    public FestivalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? keyword)
    {
        var query = _context.Festivals
            .Include(f => f.Organizer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f => f.Name.Contains(keyword));
        }

        var data = query
            .OrderBy(f => f.Date)
            .Select(f => new FestivalDetailViewModel
            {
                FestivalName = f.Name,
                Date = f.Date,
                Location = f.Location,
                OrganizerName = f.Organizer == null ? "Chưa xác định" : f.Organizer.Name,
                OrganizerType = f.Organizer == null ? "Chưa xác định" : f.Organizer.Type
            })
            .ToList();

        ViewBag.Keyword = keyword;

        return View(data);
    }

    public IActionResult Create()
    {
        LoadOrganizers();
        return View(new Festival());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Festival festival)
    {
        if (!ModelState.IsValid)
        {
            LoadOrganizers();
            return View(festival);
        }

        _context.Festivals.Add(festival);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult CreateFromJson([FromBody] Festival festival)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Festivals.Add(festival);
        _context.SaveChanges();

        return Ok(festival);
    }

    private void LoadOrganizers()
    {
        ViewBag.Organizers = new SelectList(
            _context.Organizers.OrderBy(o => o.Name).ToList(),
            "OrganizerId",
            "Name");
    }
}
