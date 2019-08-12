using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Core.Services;
using SharpBlog.Client.ViewModels;
using SharpBlog.Client.ViewModels.Account;
using SharpBlog.Core.Models;

namespace SharpBlog.Client.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller
	{
		private readonly IUserService _userService;

		public AccountController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			if (await _userService.IsNewUserRequired())
			{
				return RedirectToAction(nameof(Register));
			}
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid && await _userService.ValidateUser(model.Email, model.Password))
			{
				var user = await _userService.Get(model.Email);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Name),
					new Claim(ClaimTypes.Email, user.Email)
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
			return RedirectHome();
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Register()
		{
			if (await _userService.IsNewUserRequired())
			{
				return View();
			}

			return RedirectToAction(nameof(Login));
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (await _userService.IsNewUserRequired())
			{
				var user = new User
				{
					Email = model.Email,
					Name = model.Name,
					Password = model.Password
				};

				await _userService.RegisterUser(user);
			}

			return RedirectHome();
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectHome();
		}

		private IActionResult RedirectHome()
		{
			return RedirectToAction("Index", "Home");
		}
	}
}