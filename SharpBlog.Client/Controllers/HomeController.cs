using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Client.ViewModels;
using SharpBlog.Core.Models;
using SharpBlog.Core.Services;

namespace SharpBlog.Client.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPostService _postService;

		public HomeController(IPostService postService)
		{
			_postService = postService;
		}

		public async Task<IActionResult> Index()
		{
			List<Post> posts;
			if (User.Identity.IsAuthenticated)
			{
				posts = await _postService.GetAll();
			}
			else
			{
				posts = await _postService.GetAllPublished();
			}

			return View(posts);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
