using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApp1.Models;
using WebApp1.Services.EmailSender;
using WebApp1.Services.PhoneConfirmationService;
using WebApp1.ViewModels.Account;

namespace WebApp1.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly IPhoneConfirmationService _phoneService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender,
        IPhoneConfirmationService phoneService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _phoneService = phoneService;
    }

    /// <summary>
    ///     Try to get 2FA Authentication User.
    /// </summary>
    /// <returns>Two Factor Authentication User.</returns>
    /// <exception cref="InvalidOperationException">Unable to load two-factor authentication user.</exception>
    private async Task<User> TryExtract2FaUser()
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        return user ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    #region Logout

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "RegisterConfirmation")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Lockout

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Lockout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region AccessDenied

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    #region Login

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
        returnUrl ??= Url.Content("~/");

        if (result.Succeeded) return LocalRedirect(returnUrl);

        if (result.RequiresTwoFactor) return RedirectToAction("LoginWith2FA", new { ReturnUrl = returnUrl, vm.RememberMe, });

        if (result.IsLockedOut) return RedirectToAction("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");

        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWith2Fa(bool rememberMe, string? returnUrl = null)
    {
        try
        {
            _ = await TryExtract2FaUser();
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction("Login", new { returnUrl, });
        }

        var vm = new LoginWith2FaViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"),
            RememberMe = rememberMe,
        };

        return View(vm);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2Fa(LoginWith2FaViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        _ = await TryExtract2FaUser();

        var authenticatorCode = vm.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, vm.RememberMe, vm.RememberMachine);
        returnUrl ??= Url.Content("~/");

        if (result.Succeeded) return LocalRedirect(returnUrl);
        if (result.IsLockedOut) return RedirectToAction("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRecoveryCode(string? returnUrl = null)
    {
        try
        {
            _ = await TryExtract2FaUser();
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction("Login", new { returnUrl, });
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        _ = await TryExtract2FaUser();
        var recoveryCode = vm.RecoveryCode.Replace(" ", string.Empty);
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Content("~/"));
        if (result.IsLockedOut) return RedirectToAction("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

        return View(vm);
    }

    #endregion

    #region Register

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new User { Name = vm.Name, Surname = vm.Surname, Patronymic = vm.Patronymic, City = vm.City, };
        await _userManager.SetUserNameAsync(user, vm.Email);
        await _userManager.SetEmailAsync(user, vm.Email);
        await _userManager.SetPhoneNumberAsync(user, vm.PhoneNumber);

        var result = await _userManager.CreateAsync(user, vm.Password);
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Organizer"));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email!));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, vm.PhoneNumber);
        var ip = Request.HttpContext.Connection.RemoteIpAddress;
        _ = await _phoneService.MakePhoneCallWithToken(user.Id, user.PhoneNumber!, token, ip);

        await _signInManager.SignInAsync(user, false, "Default");

        return RedirectToAction("ConfirmPhone", new { phoneNumber = vm.PhoneNumber, });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    #endregion

    #region EmailConfirmation

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        if (userId is null || code is null) return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound($"Unable to load user with ID '{userId}'.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        await _userManager.AddClaimAsync(user, new Claim("EmailConfirmed", string.Empty));
        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = result.Succeeded ? "Почта успешно подтверждена." : "Ошибка при подтверждении почты.";

        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailChange(string? userId, string? email, string? code)
    {
        if (userId is null || email is null || code is null) return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound($"Unable to load user with ID '{userId}'.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ChangeEmailAsync(user, email, code);

        if (!result.Succeeded)
        {
            TempData["StatusMessage"] = "Error changing email.";
            return View();
        }

        var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
        if (!setUserNameResult.Succeeded)
        {
            TempData["StatusMessage"] = "Error changing user name.";
            return View();
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["StatusMessage"] = "Thank you for confirming your email change.";
        return View();
    }

    #endregion

    #region Password

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user)) return RedirectToAction("ForgotPasswordConfirmation");

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            null,
            new { area = "Identity", code, },
            Request.Scheme) ?? string.Empty;

        await _emailSender.SendEmailAsync(
            vm.Email,
            "Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? code = null)
    {
        if (code is null) return BadRequest("A code must be supplied for password reset.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var vm = new ResetPasswordViewModel { Code = code, };

        return View(vm);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null) return RedirectToAction("ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
        if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation");

        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    #endregion

    #region Phone

    [HttpGet]
    public IActionResult ConfirmPhone(string phoneNumber)
    {
        return View(new ConfirmPhoneViewModel { PhoneNumber = phoneNumber, });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPhone(ConfirmPhoneViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(vm);
        }

        var token = await _phoneService.GetConfirmationTokenForUser(user.Id, user.PhoneNumber!, vm.Code);
        var result = await _userManager.ChangePhoneNumberAsync(user, vm.PhoneNumber, token);

        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, new Claim("PhoneConfirmed", string.Empty));
            await _signInManager.SignInAsync(user, false);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code, }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email!,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

            return RedirectToAction("RegisterConfirmation");
        }

        ModelState.AddModelError(string.Empty, "Failed to verify phone number");
        return View(vm);
    }

    #endregion
}