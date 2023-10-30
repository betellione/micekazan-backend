namespace WebApp1.Controllers.Types;

public static class ManageMessageIdExtensions
{
    public static string Description(this ManageMessageId manageMessageId)
    {
        return manageMessageId switch
        {
            ManageMessageId.ChangePasswordSuccess => "Your password has been changed.",
            ManageMessageId.SetPasswordSuccess => "Your password has been set.",
            ManageMessageId.SetTwoFactorSuccess => "Your two-factor authentication provider has been set.",
            ManageMessageId.Error => "An error has occurred.",
            ManageMessageId.AddPhoneSuccess => "Your phone number was added.",
            ManageMessageId.RemovePhoneSuccess => "Your phone number was removed.",
            ManageMessageId.AddLoginSuccess => "The external login was added.",
            ManageMessageId.RemoveLoginSuccess => "The external login was removed.",
            _ => string.Empty
        };
    }
}