using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Core.Mappers;
using SharpBlog.Core.Models;
using SharpBlog.Database;

namespace SharpBlog.Core.Services.Implementation
{
	public class PostService : IPostService
	{
		private readonly BlogContext _dbContext;

		public PostService(BlogContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Post> AddOrUpdate(Post post)
		{
			if (post.Id == 0)
			{
				return await Add(post);
			}

			return await Update(post);
		}

		public Task<List<Post>> GetAllPublished()
		{
			var posts = _dbContext.Posts
                .Where(p => p.IsPublished && !p.IsDeleted)
                .Include(p => p.Comments)
				.OrderByDescending(p => p.PublicationDate)
				.Select(p => p.EntityToDto())
				.ToList();

			return Task.FromResult(posts);
		}

		public Task<List<Post>> GetAll()
		{
			var posts = _dbContext.Posts
                .Where(p => !p.IsDeleted)
                .Include(p => p.Comments)
				.OrderByDescending(p => p.PublicationDate)
				.Select(p => p.EntityToDto())
				.ToList();

			return Task.FromResult(posts);
		}

		public async Task<Post> Get(int id)
		{
			var entity = await _dbContext.Posts
                .Where(p => !p.IsDeleted)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
			return entity.EntityToDto();
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

		private async Task<Post> Add(Post post)
		{
			var dateTimeNow = DateTime.UtcNow;
			var postEntity = new Database.Models.Post
			{
				Id = post.Id,
				Author = post.Author,
				Title = post.Title,
				Content = post.Content,
				InputDate = dateTimeNow,
				IsPublished = post.IsPublished,
				LastModified = dateTimeNow,
			};

			if (post.IsPublished)
			{
				postEntity.PublicationDate = dateTimeNow;
			}

			var entity = (await _dbContext.Posts.AddAsync(postEntity)).Entity;

			await _dbContext.SaveChangesAsync();

			return entity.EntityToDto();
		}

		private async Task<Post> Update(Post post)
		{
			var dateTimeNow = DateTime.UtcNow;

			var entity = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);

			if (post.IsPublished && !entity.IsPublished)
			{
				entity.PublicationDate = dateTimeNow;
			}

			entity.Author = post.Author;
			entity.Title = post.Title;
			entity.Content = post.Content;
			entity.IsPublished = post.IsPublished;
			entity.LastModified = dateTimeNow;

			await _dbContext.SaveChangesAsync();

			return entity.EntityToDto();
		}
	}
}