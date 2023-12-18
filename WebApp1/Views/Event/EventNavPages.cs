using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp1.Views.Event;

public static class EventNavPages
{
    public const string Details = "Details";
    public const string Tickets = "Tickets";
    public const string Statistics = "Statistics";
    public const string EditPrint = "EditPrint";
    public const string EditDisplay = "EditDisplay";

    public static string DetailsNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Details);
    }

    public static string TicketsNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Tickets);
    }

    public static string StatisticsNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Statistics);
    }

    public static string EditDisplayNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, EditDisplay);
    }
    
    public static string EditPrintNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, EditPrint);
    }

    private static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        activePage ??= string.Empty;

        return activePage.Equals(page, StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
    }
}