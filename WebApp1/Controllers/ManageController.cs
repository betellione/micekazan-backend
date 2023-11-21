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
using WebApp1.Services.ClientService;
using WebApp1.Services.EventService;
using WebApp1.Services.TicketService;
using WebApp1.Services.TokenService;
using WebApp1.ViewModels.Account.Manage;

namespace WebApp1.Controllers;

[Authorize(Policy = "RegisterConfirmation")]
public class ManageController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IServiceProvider _sp;

    public ManageController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender,
        IServiceProvider sp)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _sp = sp;
    }

    /// <summary>
    ///     Get User from DB by current Claims Identity.
    /// </summary>
    /// <returns>Identity User if found.</returns>
    /// <exception cref="InvalidOperationException">User not found.</exception>
    private async Task<User> GetCurrentUserAsyncOrThrowIfNull()
    {
        return await _userManager.GetUserAsync(HttpContext.User) ?? throw new InvalidOperationException("User not found.");
    }

    #region Index

    [HttpGet]
    public async Task<IActionResult> Index(ManageMessageId? message = null)
    {
        ViewData["StatusMessage"] = message?.Description() ?? string.Empty;

        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var vm = new IndexViewModel
        {
            Username = await _userManager.GetUserNameAsync(user) ?? string.Empty,
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

        var code = await _userManager.GenerateChangeEmailTokenAsync(user, vm.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmailChange", "Account",
            new { userId = user.Id, email = vm.NewEmail, code, }, Request.Scheme)!;
        await _emailSender.SendEmailAsync(vm.NewEmail,
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

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code, }, Request.Scheme)!;
        await _emailSender.SendEmailAsync(user.Email,
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
        var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["StatusMessage"] = "Your password has been changed.";
        return RedirectToAction("ChangePassword");
    }

    #endregion

    #region Token

    [HttpGet]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> Token()
    {
        await LoadToken();
        return View();
    }

    private static async Task<bool> ImportData(Guid userId, IServiceProvider sp)
    {
        var eventService = sp.GetRequiredService<IEventService>();
        var ticketService = sp.GetRequiredService<ITicketService>();
        var clientService = sp.GetRequiredService<IClientService>();

        var importResult = await eventService.ImportEvents(userId);
        importResult &= await clientService.ImportClients(userId);
        importResult &= await ticketService.ImportTickets(userId);

        return importResult;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> Token(TokenViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadToken();
            return View(vm);
        }

        var tokenService = _sp.GetRequiredService<ITokenService>();
        var userId = new Guid(_userManager.GetUserId(User)!);
        var result = await tokenService.SetToken(userId, vm.Token);

        if (result)
        {
            _ = await ImportData(userId, _sp);
            TempData["StatusMessage"] = "Your token has been changed.\nМероприятия загружены.";
        }
        else
        {
            TempData["StatusMessage"] = "Error: Your token has not been changed. Try a different token or come back later.";
        }

        return RedirectToAction("Token");
    }

    private async Task LoadToken()
    {
        var tokenService = _sp.GetRequiredService<ITokenService>();
        var userId = new Guid(_userManager.GetUserId(User)!);
        ViewData["CurrentToken"] = await tokenService.GetToken(userId);
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
            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) is not null,
            Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
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

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            throw new InvalidOperationException(
                "Cannot generate recovery codes for user because they do not have 2FA enabled.");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCodesPost()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException("Cannot generate recovery codes for user as they do not have 2FA enabled.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, TwoFactorRecoveryCodes);

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
        var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
        if (!is2FaTokenValid)
        {
            ModelState.AddModelError(string.Empty, "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            return View();
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        TempData["StatusMessage"] = "Your authenticator app has been verified.";

        if (await _userManager.CountRecoveryCodesAsync(user) != 0) return RedirectToAction("TwoFactorAuthentication");

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, TwoFactorRecoveryCodes);
        TempData["RecoveryCodes"] = recoveryCodes?.ToList();
        return RedirectToAction("RecoveryCodes");
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(User user)
    {
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
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

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        await _signInManager.RefreshSignInAsync(user);

        TempData["StatusMessage"] = "Your authenticator app key has been reset, you will need to configure your " +
                                    "authenticator app using the new key.";

        return RedirectToAction("EnableAuthenticator");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetMachine()
    {
        await _signInManager.ForgetTwoFactorClientAsync();
        TempData["StatusMessage"] = "The current browser has been forgotten. " +
                                    "When you login again from this browser you will be prompted for your 2FA code.";
        return RedirectToAction("TwoFactorAuthentication");
    }

    [HttpGet]
    public async Task<IActionResult> DisableTwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFactorAuthenticationPost()
    {
        var user = await GetCurrentUserAsyncOrThrowIfNull();
        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        TempData["StatusMessage"] = "2FA has been disabled. You can reenable 2FA when you setup an authenticator app.";

        return RedirectToAction("TwoFactorAuthentication");
    }

    #endregion
}