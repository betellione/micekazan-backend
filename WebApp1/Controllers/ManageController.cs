using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApp1.Controllers.Types;
using WebApp1.Models;
using WebApp1.ViewModels.Account.Manage;

namespace WebApp1.Controllers;

[Authorize]
public class ManageController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
    : Controller
{
    /// <summary>
    ///     Get User from DB by current Claims Identity.
    /// </summary>
    /// <returns>Identity User if found.</returns>
    /// <exception cref="InvalidOperationException">User not found.</exception>
    private async Task<User> GetCurrentUserAsyncOrThrowIfNull()
    {
        return await userManager.GetUserAsync(HttpContext.User) ?? throw new InvalidOperationException("User not found.");
    }

    #region Index

    [HttpGet]
    public async Task<IActionResult> Index(ManageMessageId? message = null)
    {
        ViewData["StatusMessage"] = message?.Description() ?? string.Empty;

        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var vm = new IndexViewModel
        {
            Username = await userManager.GetUserNameAsync(user) ?? string.Empty,
        };

        return View(vm);
    }

    #endregion

    #region ForgetMachine

    [HttpPost]
    public IActionResult ForgetMachine()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Email

    [HttpGet]
    public async Task<IActionResult> Email()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        return View(MakeEmailViewModel(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(EmailViewModel vm)
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        if (user.Email is null) throw new Exception("User has no email");

        if (!ModelState.IsValid) return View("Email", MakeEmailViewModel(user));
        if (user.Email.Equals(vm.NewEmail, StringComparison.OrdinalIgnoreCase))
        {
            TempData["StatusMessage"] = "Your email is unchanged.";
            return RedirectToAction("Email");
        }

        var code = await userManager.GenerateChangeEmailTokenAsync(user, vm.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmailChange", "Account",
            new { userId = user.Id, email = vm.NewEmail, code, }, Request.Scheme)!;
        await emailSender.SendEmailAsync(vm.NewEmail,
            "Confirm your email",
            $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        TempData["StatusMessage"] = "Confirmation link to change email sent. Please check your email.";
        return RedirectToAction("Email");
    }

    private static EmailViewModel MakeEmailViewModel(User user)
    {
        return new EmailViewModel
        {
            Email = user.Email,
            IsEmailConfirmed = user.EmailConfirmed,
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendVerificationEmail(EmailViewModel vm)
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        if (user.Email is null) throw new Exception("User has no email");
        if (user.EmailConfirmed) throw new Exception("Email already confirmed");

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code, }, Request.Scheme)!;
        await emailSender.SendEmailAsync(user.Email,
            "Confirm your email",
            $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        TempData["StatusMessage"] = "Verification email sent. Please check your email.";
        return RedirectToAction("Email");
    }

    #endregion

    #region ChangePassword

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var result = await userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await signInManager.RefreshSignInAsync(user);

        TempData["StatusMessage"] = "Your password has been changed.";
        return RedirectToAction("ChangePassword");
    }

    #endregion

    #region TwoFactorAuthentication

    [HttpGet]
    public IActionResult TwoFactorAuthentication()
    {
        var vm = new TwoFactorAuthenticationViewModel();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetAuthenticatorKey()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        await userManager.ResetAuthenticatorKeyAsync(user);

        return RedirectToAction("Index", "Manage");
    }

    [HttpGet]
    public IActionResult ShowRecoveryCodes(ShowRecoveryCodesViewModel? vm)
    {
        return vm is null || vm.RecoveryCodes.Count == 0 ? RedirectToAction("TwoFactorAuthentication") : View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCode()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var codes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5))?.ToArray() ?? Array.Empty<string>();
        return View("ShowRecoveryCodes", new ShowRecoveryCodesViewModel { RecoveryCodes = codes, });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task EnableTwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        await userManager.SetTwoFactorEnabledAsync(user, true);
        await signInManager.SignInAsync(user, false);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task DisableTwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        await userManager.SetTwoFactorEnabledAsync(user, false);
        await signInManager.SignInAsync(user, false);
    }

    #endregion
}