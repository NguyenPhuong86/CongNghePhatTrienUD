using AppDemo.Data;
using AppDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppDemo.Controllers;

public class OrganizersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrganizersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? keyword)
    {
        var query = _context.Organizers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(o => o.Name.Contains(keyword));
        }

        ViewBag.Keyword = keyword;

        return View(query.OrderBy(o => o.Name).ToList());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Organizer());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Organizer organizer)
    {
        if (!ModelState.IsValid)
        {
            return View(organizer);
        }

        _context.Organizers.Add(organizer);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
