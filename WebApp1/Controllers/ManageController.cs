using System.Globalization;
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
    
    #region Token

    [HttpGet]
    public IActionResult Token()
    {
        return View();
    }

    [Authorize(Roles = "Organizer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Token(TokenViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await GetCurrentUserAsyncOrThrowIfNull();
        user.Tokens.Add(new CreatorToken());

        TempData["StatusMessage"] = "Your token has been changed.";
        return RedirectToAction("Token");
    }

    #endregion

    #region TwoFactorAuthentication

    private const int TwoFactorRecoveryCodes = 8;

    [HttpGet]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var vm = new TwoFactorAuthenticationViewModel
        {
            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) is not null,
            Is2FaEnabled = await userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult RecoveryCodes()
    {
        var codes = (IList<string>)(TempData["RecoveryCodes"] ?? Array.Empty<string>());
        return codes.Count == 0 ? RedirectToAction("TwoFactorAuthentication") : View();
    }

    [HttpGet]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();

        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
            throw new InvalidOperationException(
                "Cannot generate recovery codes for user because they do not have 2FA enabled.");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCodesPost()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot generate recovery codes for user as they do not have 2FA enabled.");

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, TwoFactorRecoveryCodes);

        TempData["RecoveryCodes"] = recoveryCodes?.ToList();
        TempData["StatusMessage"] = "You have generated new recovery codes.";

        return RedirectToAction("RecoveryCodes");
    }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        await LoadSharedKeyAndQrCodeUriAsync(user);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel vm)
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return View();
        }

        var verificationCode = vm.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
            userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
        if (!is2FaTokenValid)
        {
            ModelState.AddModelError(string.Empty, "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return View();
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        TempData["StatusMessage"] = "Your authenticator app has been verified.";

        if (await userManager.CountRecoveryCodesAsync(user) != 0) return RedirectToAction("TwoFactorAuthentication");

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, TwoFactorRecoveryCodes);
        TempData["RecoveryCodes"] = recoveryCodes?.ToList();
        return RedirectToAction("RecoveryCodes");
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(User user)
    {
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        ViewData["SharedKey"] = FormatKey(unformattedKey!);
        ViewData["AuthenticatorUri"] = GenerateQrCodeUri(user.Email!, unformattedKey!);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;

        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length) result.Append(unformattedKey.AsSpan(currentPosition));

        return result.ToString().ToLowerInvariant();
    }

    private static string GenerateQrCodeUri(string email, string unformattedKey)
    {
        const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        return string.Format(
            CultureInfo.InvariantCulture,
            format,
            UrlEncoder.Default.Encode("Micekazan"),
            UrlEncoder.Default.Encode(email),
            unformattedKey);
    }

    [HttpGet]
    public IActionResult ResetAuthenticatorKey()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetAuthenticatorKeyPost()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        await signInManager.RefreshSignInAsync(user);

        TempData["StatusMessage"] = "Your authenticator app key has been reset, you will need to configure your " +
                                    "authenticator app using the new key.";

        return RedirectToAction("EnableAuthenticator");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetMachine()
    {
        await signInManager.ForgetTwoFactorClientAsync();
        TempData["StatusMessage"] = "The current browser has been forgotten. " +
                                    "When you login again from this browser you will be prompted for your 2FA code.";
        return RedirectToAction("TwoFactorAuthentication");
    }

    [HttpGet]
    public async Task<IActionResult> DisableTwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFactorAuthenticationPost()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var result = await userManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");

        TempData["StatusMessage"] = "2FA has been disabled. You can reenable 2FA when you setup an authenticator app.";

        return RedirectToAction("TwoFactorAuthentication");
    }

    #endregion
}