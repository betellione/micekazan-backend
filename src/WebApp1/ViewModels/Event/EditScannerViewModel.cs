using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp1.ViewModels.Event;

public class EditScannerViewModel
{
    public Guid Id { get; set; }

    [DisplayName("Токен")]
    [MinLength(32)]
    [MaxLength(256)]
    [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Токен имеет неверный формат. Разрешены только цифры и латинские буквы")]
    public string? Token { get; set; }

    [Display(Name = "Шаблон печати")]
    public IEnumerable<SelectListItem> TemplateIds { get; set; } = Enumerable.Empty<SelectListItem>();

    public string? SelectedTemplateId { get; set; }

    [DisplayName("Является ли постаматом?")]
    public bool IsAutomate { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public string? EventName { get; set; }
    public long? EventId { get; set; }
}