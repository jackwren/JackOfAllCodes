using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RegisterRequest = JackOfAllCodes.Web.Models.ViewModels.RegisterRequest;
using JackOfAllCodes.Web.Models.ViewModels;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.RegisterAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["ErrorMessage"] = result.Message;
                return View(request);
            }

            return RedirectToAction("RegistrationSuccess");
        }

        ViewData["ErrorMessage"] = "Password does not meet validation criteria.";
        return View(request);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.LoginAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["ErrorMessage"] = result.Message;
                return View(request);
            }

            // Create cookie claims for the session.
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, request.Email),
                new (ClaimTypes.NameIdentifier, result.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Profile");
        }

        ViewData["ErrorMessage"] = "Validation failed";
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ProfileAsync()
    {
        if (User.Identity != null && !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }

        var user = await _accountService.GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View(user);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromForm] string profilePictureUrl)
    {
        var user = await _accountService.GetCurrentUser();

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        user.ProfilePictureUrl = profilePictureUrl;

        // Update userProfile
        await _accountService.UpdateUser(user);

        return RedirectToAction("Profile");
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var callbackUrl = Url.Action(
            "SetNewPassword",
            "Account",
            new { email = model.Email },
            protocol: Request.Scheme
        );

        var result = await _accountService.ResetUserPassword(model.Email, callbackUrl);

        return RedirectToAction("ResetPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult SetNewPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var model = new SetNewPasswordViewModel
        {
            Token = token,
            Email = email
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SetNewPassword(SetNewPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.SetNewUserPassword(model);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            ViewData["ErrorMessage"] = result.Message;
            return View(model);
        }

        TempData["SuccessMessage"] = "Your password has been reset successfully!";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult RegistrationSuccess()
    {
        return View();
    }
}