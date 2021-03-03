using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SharpBlog.Client.Attributes;
using SharpBlog.Client.Services;
using SharpBlog.Client.ViewModels;
using SharpBlog.Core.Dal;
using SharpBlog.Core.Dto;

namespace SharpBlog.Client.Controllers
{
    [NewUserRedirection]
    public class BlogController : Controller
    {
        private readonly IPostDal _posDal;
        private readonly ICommentDal _commentDal;
        private readonly ISettingsService _settingsService;

        public BlogController(
            IPostDal postDal,
            ICommentDal commentDal,
            ISettingsService settingsService)
        {
            _posDal = postDal;
            _commentDal = commentDal;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _posDal.GetAll();

            ViewData["BlogTitle"] = _settingsService.GetBlogName();
            ViewData["BlogDescription"] = _settingsService.GetBlogDescription();
            return View(posts);
        }

        public async Task<IActionResult> Post(int id)
        {
            var post = await _posDal.Get(id);

            ViewData["BlogTitle"] = $"{post?.Title}";
            ViewData["BlogDescription"] = post?.Content?.StripHTML()?.Excerpt(30);
            return View(new PostViewModel(post));
        }

        public async Task<IActionResult> Category(string name)
        {
            var posts = await _posDal.GetByCategory(name);

            ViewData["category"] = name;
            ViewData["BlogTitle"] = $"{_settingsService.GetBlogName()} - {name}";
            ViewData["BlogDescription"] = $"Blog posts in the {name} category";
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

            var post = await _posDal.Get((int)id);
            var editPost = new PostFormViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                IsPublished = post.IsPublished,
                Categories = string.Join(" ", post.Categories.Select(t => t.Name))
            };

            ViewData["BlogTitle"] = $"{post?.Title}";
            ViewData["BlogDescription"] = post?.Content?.StripHTML()?.Excerpt(30);
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

            var addedPost = await _posDal.AddOrUpdate(new PostDto
            {
                Id = post.Id,
                Author = User.Identity.Name,
                Title = post.Title,
                Content = post.Content,
                IsPublished = post.IsPublished,
                Categories = post.Categories?
                                    .Split(" ")
                                    .Where(c => !string.IsNullOrWhiteSpace(c))
                                    .Select(c => new CategoryDto { Name = c })
            });

            return RedirectToAction(nameof(Post), new { id = addedPost.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _posDal.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentFormViewModel comment)
        {
            if (_settingsService.GetReCaptchaEnabled())
            {
                var reCaptchaIsValid = await VerifyReCaptchaToken(HttpContext.Request.Form["g-recaptcha-response"],
                                                                    _settingsService.GetReCaptchaPrivateKey());
                if (!reCaptchaIsValid)
                {
                    ModelState.TryAddModelError("reCaptchaValidationFailed", "You are a robot!");
                }
            }

            if (!ModelState.IsValid)
            {
                var post = await _posDal.Get(comment.PostId);
                return View(nameof(Post), new PostViewModel(post, comment));
            }

            await _commentDal.Add(new CommentDto
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
            await _commentDal.Delete(id);
            return RedirectToAction(nameof(Post), new { id = postId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<bool> VerifyReCaptchaToken(string token, string privateKey)
        {
            var client = new HttpClient();
            var formData = new FormUrlEncodedContent(
                                new Dictionary<string, string>
                                {
                                    { "response", token },
                                    { "secret", privateKey }
                                });
            var res = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", formData);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            string JSONres = await res.Content.ReadAsStringAsync();
            var JSONdata = JObject.Parse(JSONres);

            bool.TryParse(JSONdata.GetValue("success").ToString(), out var success);

            return success;
        }
    }
}