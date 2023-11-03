using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp1.Views.Manage;

public static class ManageNavPages
{
    public const string Profile = "Profile";
    public const string Email = "Email";
    public const string ChangePassword = "ChangePassword";
    public const string TwoFactorAuthentication = "TwoFactorAuthentication";
    public const string ChangeToken = "ChangeToken";

    public static string ProfileNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Profile);
    }

    public static string EmailNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, Email);
    }

    public static string ChangePasswordNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, ChangePassword);
    }

    public static string TwoFactorAuthenticationNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, TwoFactorAuthentication);
    }
    
    public static string ChangeTokenNavClass(ViewContext viewContext)
    {
        return PageNavClass(viewContext, ChangeToken);
    }

    private static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        activePage ??= string.Empty;

        return activePage.Equals(page, StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
    }
}