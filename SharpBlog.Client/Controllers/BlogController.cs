using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Client.ViewModels;
using SharpBlog.Core.Models;
using SharpBlog.Core.Services;

namespace SharpBlog.Client.Controllers
{
    public class BlogController : Controller
    {
	    private readonly IPostService _postService;
	    private readonly ICommentService _commentService;

	    public BlogController(
		    IPostService postService,
			ICommentService commentService)
		{
			_postService = postService;
			_commentService = commentService;
		}

        public async Task<IActionResult> Index(int id)
        {
			var post = await _postService.Get(id);
			return View(new PostViewModel(post));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditPost(int? id)
        {
	        if (id == null)
	        {
		        return View(new PostFormViewModel());
	        }

			var post = await _postService.Get((int)id);
	        var editPost = new PostFormViewModel
			{
				Id = post.Id,
				Title = post.Title,
				Content = post.Content,
				IsPublished = post.IsPublished,
				Categories = string.Join(" ", post.Categories.Select(t => t.Name))
			};

	        return View(editPost);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPost(PostFormViewModel post)
		{
			if (!ModelState.IsValid)
			{
				return View(post);
			}

			var addedPost = await _postService.AddOrUpdate(new PostDto
			{
				Id = post.Id,
				Author = User.Identity.Name,
				Title = post.Title,
				Content = post.Content,
				IsPublished = post.IsPublished,
				Categories = post.Categories.Split(" ").Select(t => new CategoryDto { Name = t })
			});

			return RedirectToAction(nameof(Index), new { id = addedPost.Id });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePost(int id)
		{
			await _postService.Delete(id);
			return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentFormViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                var post = await _postService.Get(comment.PostId);
                return View("Index", new PostViewModel(post, comment));
            }

            await _commentService.Add(new CommentDto
            {
                PostId = comment.PostId,
                Author = comment.Name,
                Email = comment.Email,
                Content = comment.Content
            });

            return RedirectToAction("Index", new { id = comment.PostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            await _commentService.Delete(id);
            return RedirectToAction("Index", new { id = postId });
        }
    }
}