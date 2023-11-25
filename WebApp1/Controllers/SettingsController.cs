using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data.Stores;
using WebApp1.Enums;
using WebApp1.Mapping;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.Services.TemplateService;
using WebApp1.ViewModels.Settings;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer")]
public class SettingsController : Controller
{
    private readonly ITemplateService _templateService;
    private readonly IEventService _eventService;
    private readonly IScreenStore _screenStore;
    private readonly UserManager<User> _userManager;

    public SettingsController(ITemplateService templateService, IEventService eventService, IScreenStore screenStore, UserManager<User> userManager)
    {
        _screenStore = screenStore;
        _templateService = templateService;
        _eventService = eventService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(long? templateId = null)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var template = templateId is null
            ? new TemplateViewModel()
            : (await _templateService.GetTemplate(templateId.Value))?.MapToViewModel();

        var vm = new PrintViewModel
        {
            TemplateViewModel = template ?? new TemplateViewModel(),
            TemplateIds = await _templateService.GetTemplateIds(userId),
            SelectedTemplateId = templateId,
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Display(long eventId)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var selectedEvent = await _eventService.GetById(eventId);
        var events = await _eventService.GetAll(userId);
        var waiting = _screenStore.GetScreenByType(eventId, ScreenTypes.Waiting);
        var vm = new DisplayViewModel()
        {
            Events = events,
            SelectedEventId = selectedEvent?.Id,
        };
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTemplateAsNew(TemplateViewModel vm)
    {
        if (!ModelState.IsValid) return View("Index", new PrintViewModel { TemplateViewModel = vm, });

        var userId = new Guid(_userManager.GetUserId(User)!);
        await _templateService.AddTemplate(userId, vm);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTemplate(TemplateViewModel vm)
    {
        if (!ModelState.IsValid) return View("Index", new PrintViewModel { TemplateViewModel = vm, });

        await _templateService.UpdateTemplate(vm);

        return RedirectToAction("Index", new { templateId = vm.Id, });
    }
}