using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Client.ViewModels.Account;
using SharpBlog.Common.Models;
using SharpBlog.Client.Attributes;
using SharpBlog.Client.Services;
using SharpBlog.Common.Dal;

namespace SharpBlog.Client.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserDal _userDal;
        private readonly IUserService _userService;
        private readonly ISettingsService _settingsService;

        public AccountController(
            IUserDal userDal,
            IUserService userService,
            ISettingsService settingsService)
        {
            _userDal = userDal;
            _userService = userService;
            _settingsService = settingsService;
        }

        [HttpGet]
        [AllowAnonymous]
        [NewUserRedirection]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid && await _userDal.ValidateUser(model.Email, model.Password))
            {
                var user = await _userDal.GetAdmin();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("LastPasswordChangeDate", user.LastPasswordChangeDate.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principle = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddYears(1),
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle, properties);

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Username or password is invalid.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectHomePage();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterCompleted()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _userService.RegisterUser(model);

            return RedirectToAction(nameof(RegisterCompleted));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isPasswordChanged = await _userDal.ChangePassword(model.OldPassword, model.NewPassword, model.ConfirmNewPassword);
            if (isPasswordChanged)
            {
                return RedirectLoginPage();
            }

            ModelState.TryAddModelError("changePasswordError", "Change password error, provide old password and make sure confirmation form is valid");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageAccount()
        {
            var user = await _userDal.GetAdmin();
            var model = new ManageAccountViewModel
            {
                Name = user.Name,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageAccount(ManageAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _userDal.UpdateUser(model.Name, model.Email);

            ViewData["accountDetailsSaved"] = "Account details has been saved";
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectHomePage();
        }

        private IActionResult RedirectHomePage()
        {
            return RedirectToAction(nameof(BlogController.Index), "Blog");
        }

        private IActionResult RedirectLoginPage()
        {
            return RedirectToAction(nameof(Login));
        }
    }
}