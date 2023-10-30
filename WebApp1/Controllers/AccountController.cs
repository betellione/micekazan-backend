using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApp1.Models;
using WebApp1.ViewModels.Account;

namespace WebApp1.Controllers;

[Authorize]
public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
    : Controller
{
    /*[HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return View("Error");
        var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
        var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
            .ToList();
        return View(new SendCodeViewModel
            { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return View("Error");
        return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result =
            await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                model.RememberBrowser);
        if (result.Succeeded) return RedirectToLocalOrIndex(model.ReturnUrl);
        if (result.IsLockedOut)
        {
            _logger.LogWarning(7, "User account locked out.");
            return View("Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid code.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string returnUrl = null)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return View("Error");
        return View(new VerifyAuthenticatorCodeViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyAuthenticatorCode(VerifyAuthenticatorCodeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result =
            await signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe, model.RememberBrowser);
        if (result.Succeeded) return RedirectToLocalOrIndex(model.ReturnUrl);
        if (result.IsLockedOut)
        {
            _logger.LogWarning(7, "User account locked out.");
            return View("Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid code.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> UseRecoveryCode(string returnUrl = null)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return View("Error");
        return View(new UseRecoveryCodeViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UseRecoveryCode(UseRecoveryCodeViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);
        if (result.Succeeded) return RedirectToLocalOrIndex(model.ReturnUrl);

        ModelState.AddModelError(string.Empty, "Invalid code.");
        return View(model);
    }*/

    /// <summary>
    ///     Try to get 2FA Authentication User.
    /// </summary>
    /// <returns>Two Factor Authentication User.</returns>
    /// <exception cref="InvalidOperationException">Unable to load two-factor authentication user.</exception>
    private async Task<User> TryExtract2FaUser()
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        return user ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    #region Logout

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Lockout

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Lockout()
    {
        return View();
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

        var result = await signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
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
        _ = await TryExtract2FaUser();

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
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, vm.RememberMe,
            vm.RememberMachine);
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
        _ = await TryExtract2FaUser();
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
        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Content("~/"));
        if (result.IsLockedOut) return RedirectToAction("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

        return View(vm);
    }

    [HttpGet]
    public IActionResult LoinWithRecoveryCode()
    {
        throw new NotImplementedException();
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

        var user = new User();
        await userManager.SetUserNameAsync(user, vm.Email);
        await userManager.SetEmailAsync(user, vm.Email);

        var result = await userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId, code, }, Request.Scheme);

        await emailSender.SendEmailAsync(
            vm.Email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

        return RedirectToAction("RegisterConfirmation");
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

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound($"Unable to load user with ID '{userId}'.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await userManager.ConfirmEmailAsync(user, code);
        TempData["StatusMessage"] = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailChange(string? userId, string? email, string? code)
    {
        if (userId is null || email is null || code is null) return RedirectToAction("Index", "Home");

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound($"Unable to load user with ID '{userId}'.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await userManager.ChangeEmailAsync(user, email, code);

        if (!result.Succeeded)
        {
            TempData["StatusMessage"] = "Error changing email.";
            return View();
        }

        var setUserNameResult = await userManager.SetUserNameAsync(user, email);
        if (!setUserNameResult.Succeeded)
        {
            TempData["StatusMessage"] = "Error changing user name.";
            return View();
        }

        await signInManager.RefreshSignInAsync(user);

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

        var user = await userManager.FindByEmailAsync(vm.Email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return RedirectToAction("ForgotPasswordConfirmation");

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            null,
            new { area = "Identity", code, },
            Request.Scheme) ?? string.Empty;

        await emailSender.SendEmailAsync(
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

        var user = await userManager.FindByEmailAsync(vm.Email);
        if (user is null) return RedirectToAction("ResetPasswordConfirmation");

        var result = await userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
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
}