using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Client.Services;
using SharpBlog.Client.ViewModels;
using SharpBlog.Core.Models;
using SharpBlog.Core.Services;

namespace SharpBlog.Client.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ISettingsService _settingsService;

        public BlogController(
            IPostService postService,
            ICommentService commentService,
            ISettingsService settingsService)
        {
            _postService = postService;
            _commentService = commentService;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAll();

            ViewData["BlogTitle"] = _settingsService.GetBlogName();
            ViewData["BlogDescription"] = _settingsService.GetBlogDescription();
            return View(posts);
        }

        public async Task<IActionResult> Post(int id)
        {
            var post = await _postService.Get(id);

            ViewData["BlogTitle"] = $"{post?.Title}";
            ViewData["BlogDescription"] = _settingsService.GetBlogDescription(); // TODO: description for post (Excerpt)
            return View(new PostViewModel(post));
        }

        public async Task<IActionResult> Category(string name)
        {
            var posts = await _postService.GetByCategory(name);

            ViewData["category"] = name;
            ViewData["BlogTitle"] = $"{_settingsService.GetBlogName()} - {name}";
            ViewData["BlogDescription"] = $"Blog posts in the {_settingsService.GetBlogDescription()} category";
            return View(posts);
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

            ViewData["BlogTitle"] = $"{post?.Title}";
            ViewData["BlogDescription"] = _settingsService.GetBlogDescription(); // TODO: description for post (Excerpt)
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
                Categories = post.Categories?.Split(" ").Select(t => new CategoryDto { Name = t })
            });

            return RedirectToAction(nameof(Post), new { id = addedPost.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentFormViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                var post = await _postService.Get(comment.PostId);
                return View(nameof(Post), new PostViewModel(post, comment));
            }

            await _commentService.Add(new CommentDto
            {
                PostId = comment.PostId,
                Author = comment.Name,
                Email = comment.Email,
                Content = comment.Content
            });

            return RedirectToAction(nameof(Post), new { id = comment.PostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            await _commentService.Delete(id);
            return RedirectToAction(nameof(Post), new { id = postId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}