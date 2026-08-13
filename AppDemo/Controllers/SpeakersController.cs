using AppDemo.Data;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Controllers;

public class SpeakersController : Controller
{
    private const int PageSize = 5;
    private readonly ApplicationDbContext _context;

    public SpeakersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? keyword, string sortOrder = "name_asc", int page = 1)
    {
        page = Math.Max(page, 1);
        var query = _context.Speakers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s => s.Name.Contains(keyword));
        }

        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(s => s.Name),
            "title_asc" => query.OrderBy(s => s.Title),
            "title_desc" => query.OrderByDescending(s => s.Title),
            _ => query.OrderBy(s => s.Name)
        };

        var totalItems = query.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)PageSize));
        page = Math.Min(page, totalPages);

        var model = new PagedResult<Speaker>
        {
            Items = query.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
            Keyword = keyword,
            SortOrder = sortOrder,
            Page = page,
            TotalPages = totalPages
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Speaker());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Speaker speaker)
    {
        if (!ModelState.IsValid)
        {
            return View(speaker);
        }

        _context.Speakers.Add(speaker);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var speaker = _context.Speakers.Find(id);
        if (speaker == null)
        {
            return NotFound();
        }

        return View(speaker);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Speaker speaker)
    {
        if (id != speaker.SpeakerId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(speaker);
        }

        _context.Speakers.Update(speaker);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var speaker = _context.Speakers
            .Include(s => s.Presentations)
            .FirstOrDefault(s => s.SpeakerId == id);

        if (speaker == null)
        {
            return NotFound();
        }

        return View(speaker);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var speaker = _context.Speakers.Find(id);
        if (speaker == null)
        {
            return NotFound();
        }

        _context.Speakers.Remove(speaker);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
