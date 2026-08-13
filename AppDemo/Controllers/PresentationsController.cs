using AppDemo.Data;
using AppDemo.Models;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Controllers;

public class PresentationsController : Controller
{
    private const int PageSize = 5;
    private readonly ApplicationDbContext _context;
    public PresentationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? keyword, string sortOrder = "topic_asc", int page = 1)
    {
        page = Math.Max(page, 1);
        var query = _context.Presentations
            .Include(p => p.Speaker)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p =>
                p.Topic.Contains(keyword) ||
                p.Speaker != null && p.Speaker.Name.Contains(keyword));
        }

        query = sortOrder switch
        {
            "topic_desc" => query.OrderByDescending(p => p.Topic),
            "duration_asc" => query.OrderBy(p => p.Duration),
            "duration_desc" => query.OrderByDescending(p => p.Duration),
            _ => query.OrderBy(p => p.Topic)
        };

        var totalItems = query.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)PageSize));
        page = Math.Min(page, totalPages);

        var model = new PagedResult<PresentationListViewModel>
        {
            Items = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new PresentationListViewModel
                {
                    PresentationId = p.PresentationId,
                    Topic = p.Topic,
                    Duration = p.Duration,
                    Slides = p.Slides,
                    SpeakerName = p.Speaker == null ? "Chưa xác định" : p.Speaker.Name,
                    SpeakerTitle = p.Speaker == null ? "Chưa xác định" : p.Speaker.Title
                })
                .ToList(),
            Keyword = keyword,
            SortOrder = sortOrder,
            Page = page,
            TotalPages = totalPages
        };

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = new PresentationEditViewModel();
        await PrepareModelAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PresentationEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PrepareModelAsync(model, cancellationToken);
            return View(model);
        }

        if (!await SpeakerExistsAsync(model.SpeakerId, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.SpeakerId), "Không tìm thấy diễn giả đã chọn.");
            await PrepareModelAsync(model, cancellationToken);
            return View(model);
        }

        var presentation = MapToEntity(model);
        _context.Presentations.Add(presentation);
        await _context.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var presentation = await _context.Presentations.FindAsync([id], cancellationToken);
        if (presentation == null)
        {
            return NotFound();
        }

        var model = new PresentationEditViewModel
        {
            PresentationId = presentation.PresentationId,
            Topic = presentation.Topic,
            Duration = presentation.Duration,
            Slides = presentation.Slides,
            SpeakerId = presentation.SpeakerId,
            Description = presentation.Description
        };
        await PrepareModelAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PresentationEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.PresentationId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PrepareModelAsync(model, cancellationToken);
            return View(model);
        }

        var presentation = await _context.Presentations.FindAsync([id], cancellationToken);
        if (presentation is null)
        {
            return NotFound();
        }

        if (!await SpeakerExistsAsync(model.SpeakerId, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.SpeakerId), "Không tìm thấy diễn giả đã chọn.");
            await PrepareModelAsync(model, cancellationToken);
            return View(model);
        }

        presentation.Topic = model.Topic;
        presentation.Duration = model.Duration;
        presentation.Slides = model.Slides;
        presentation.SpeakerId = model.SpeakerId;
        presentation.Description = model.Description;
        await _context.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var presentation = _context.Presentations
            .Include(p => p.Speaker)
            .FirstOrDefault(p => p.PresentationId == id);

        if (presentation == null)
        {
            return NotFound();
        }

        return View(presentation);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var presentation = _context.Presentations.Find(id);
        if (presentation == null)
        {
            return NotFound();
        }

        _context.Presentations.Remove(presentation);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareModelAsync(
        PresentationEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.SpeakerOptions = await _context.Speakers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem(s.Name, s.SpeakerId.ToString()))
            .ToListAsync(cancellationToken);
    }

    private Task<bool> SpeakerExistsAsync(int speakerId, CancellationToken cancellationToken) =>
        _context.Speakers.AnyAsync(s => s.SpeakerId == speakerId, cancellationToken);

    private static Presentation MapToEntity(PresentationEditViewModel model) => new()
    {
        Topic = model.Topic,
        Duration = model.Duration,
        Slides = model.Slides,
        SpeakerId = model.SpeakerId,
        Description = model.Description
    };
}
