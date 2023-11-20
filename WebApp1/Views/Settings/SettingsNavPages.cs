using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp1.Views.Settings;

public static class SettingsNavPages
{
    public const string Print = "Print";
    public const string Display = "Display";

    public static string PrintNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Print);
    }

    public static string DisplayNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Display);
    }

    private static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        activePage ??= string.Empty;

        return activePage.Equals(page, StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
    }
}