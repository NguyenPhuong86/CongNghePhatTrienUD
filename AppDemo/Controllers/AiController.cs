using AppDemo.Data;
using AppDemo.Models;
using AppDemo.Services;
using AppDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Controllers;

public class AiController : Controller
{
    private readonly IAiTextService _aiTextService;
    private readonly ApplicationDbContext _context;

    public AiController(IAiTextService aiTextService, ApplicationDbContext context)
    {
        _aiTextService = aiTextService;
        _context = context;
    }

    public IActionResult Index()
    {
        return View(CreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuggestDescription(
        AiAssistantViewModel model,
        CancellationToken cancellationToken)
    {
        model.Presentations = LoadPresentations();
        model.IsAiConfigured = _aiTextService.IsConfigured;

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var result = await _aiTextService.SuggestPresentationDescriptionAsync(
            model.Topic,
            model.SpeakerName,
            cancellationToken);
        model.Result = result.Text;
        model.ResultSource = result.Source;
        model.ResultErrorCategory = result.ErrorCategory;

        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Summarize(CancellationToken cancellationToken)
    {
        var model = CreateModel();
        var items = model.Presentations.Select(p =>
            $"{p.Topic} - {p.Duration} phút - {p.Speaker?.Name}");

        var result = await _aiTextService.SummarizePresentationsAsync(
            items,
            cancellationToken);
        model.Summary = result.Text;
        model.SummarySource = result.Source;
        model.SummaryErrorCategory = result.ErrorCategory;
        return View("Index", model);
    }

    private AiAssistantViewModel CreateModel()
    {
        return new AiAssistantViewModel
        {
            IsAiConfigured = _aiTextService.IsConfigured,
            Presentations = LoadPresentations()
        };
    }

    private List<Presentation> LoadPresentations()
    {
        return _context.Presentations
            .Include(p => p.Speaker)
            .AsNoTracking()
            .OrderBy(p => p.Topic)
            .ToList();
    }
}
