using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp1.Views.Event;

public static class EventNavPages
{
    public const string Details = "Details";

    public static string DetailsNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Details);
    }

    private static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        activePage ??= string.Empty;

        return activePage.Equals(page, StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
    }
}