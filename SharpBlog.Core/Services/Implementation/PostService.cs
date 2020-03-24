using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Core.Mappers;
using SharpBlog.Core.Models;
using SharpBlog.Database;
using SharpBlog.Database.Comparers;
using SharpBlog.Database.Models;

namespace SharpBlog.Core.Services.Implementation
{
	public class PostService : IPostService
	{
		private readonly BlogContext _dbContext;
		private readonly IHttpContextAccessor _contextAccessor;

		public PostService(
			BlogContext dbContext,
			IHttpContextAccessor contextAccessor)
		{ 
			_dbContext = dbContext;
			_contextAccessor = contextAccessor;
		}

		public async Task<PostDto> AddOrUpdate(PostDto post)
		{
			if (post.Id == 0)
			{
				return await Add(post);
			}

			return await Update(post);
		}

		public Task<List<PostDto>> GetAll()
		{
			var posts = _dbContext.Posts
                .Where(p => (p.IsPublished || IsAdmin()) && !p.IsDeleted)
                .Include(p => p.Comments)
				.Include(p => p.PostCategory)
					.ThenInclude(p => p.Category)
				.OrderByDescending(p => p.InputDate)
				.ThenByDescending(p => p.PublicationDate)
				.Select(p => p.ToDto())
				.ToList();

			return Task.FromResult(posts);
		}

		public async Task<PostDto> Get(int id)
		{
			var entity = await _dbContext.Posts
                .Where(p => (p.IsPublished || IsAdmin()) && !p.IsDeleted)
                .Include(p => p.Comments)
				.Include(p => p.PostCategory)
					.ThenInclude(p => p.Category)
				.FirstOrDefaultAsync(p => p.Id == id);

			return entity?.ToDto();
		}

		public async Task Delete(int id)
		{
			var entity = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
			if (entity != null)
			{
                entity.IsDeleted = true;
                _dbContext.Update(entity);
				await _dbContext.SaveChangesAsync();
			}
		}

		private async Task<PostDto> Add(PostDto post)
		{
			var dateTimeNow = DateTime.UtcNow;
			var postCategories = GetPostCategoryEntities(post);
			var postEntity = new Post
			{
				Id = post.Id,
				Author = post.Author,
				Title = post.Title,
				Content = post.Content,
				InputDate = dateTimeNow,
				IsPublished = post.IsPublished,
				LastModified = dateTimeNow,
				PostCategory = postCategories.ToList()
			};

			if (post.IsPublished)
			{
				postEntity.PublicationDate = dateTimeNow;
			}

			var entity = (await _dbContext.Posts.AddAsync(postEntity)).Entity;

			await _dbContext.SaveChangesAsync();

			return entity.ToDto();
		}

		private async Task<PostDto> Update(PostDto post)
		{
			var dateTimeNow = DateTime.UtcNow;
			var postCategories = GetPostCategoryEntities(post);
			var entity = await _dbContext
				.Posts
				.Include(p => p.PostCategory)
				.FirstOrDefaultAsync(p => p.Id == post.Id);

			if (post.IsPublished && !entity.IsPublished)
			{
				entity.PublicationDate = dateTimeNow;
			}

			entity.Author = post.Author;
			entity.Title = post.Title;
			entity.Content = post.Content;
			entity.IsPublished = post.IsPublished;
			entity.LastModified = dateTimeNow;
			entity.PostCategory = postCategories.ToList();

			await _dbContext.SaveChangesAsync();

			return entity.ToDto();
		}

		private IEnumerable<PostCategory> GetPostCategoryEntities(PostDto post)
		{
			if(post?.Categories == null)
			{
				return new List<PostCategory>();
			}

			var allCategories = _dbContext.Categories.ToList();
			var categories = post.Categories.Select(c => new Category { Name = c.Name });

			var existingCategories = allCategories.Intersect(categories, new CategoryEqualityComparer());
			var missingCategories = categories.Except(allCategories, new CategoryEqualityComparer());

			var postCategories = existingCategories
				.Concat(missingCategories)
				.Select(c => new PostCategory {
					Category = c
				});

			return postCategories;
		}

		private bool IsAdmin() => _contextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
	}
}