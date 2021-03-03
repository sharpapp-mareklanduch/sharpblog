using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Core.Mappers;
using SharpBlog.Core.Dto;
using SharpBlog.Database;
using SharpBlog.Database.Comparers;
using SharpBlog.Database.Models;

namespace SharpBlog.Core.Dal.Implementation
{
    public class PostDal : IPostDal
    {
        private readonly BlogContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public PostDal(
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

        public Task<IEnumerable<PostDto>> GetAll()
        {
            var posts = _dbContext.Posts
                .Where(p => (p.IsPublished || IsAdmin()) && !p.IsDeleted)
                .Include(p => p.Comments)
                .Include(p => p.PostCategory)
                    .ThenInclude(p => p.Category)
                .OrderByDescending(p => IsAdmin() ? p.LastModified : p.PublicationDate)
                .Select(p => p.ToDto())
                .AsEnumerable();

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

        public Task<IEnumerable<PostDto>> GetByCategory(string name)
        {
            var posts = _dbContext.Posts
                .Where(p => (p.IsPublished || IsAdmin()) && !p.IsDeleted)
                .Include(p => p.Comments)
                .Include(p => p.PostCategory)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.PostCategory.Select(pc => pc.Category.Name.ToLower()).Contains(name.ToLower()))
                .OrderByDescending(p => IsAdmin() ? p.LastModified : p.PublicationDate)
                .Select(p => p.ToDto())
                .AsEnumerable();

            return Task.FromResult(posts);
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
            const string unpublished = "unpublished";

            var allCategories = _dbContext.Categories.ToList();
            var postCategories = post.Categories?.Select(c => new Category { Name = c.Name }).ToList() ?? new List<Category>();

            var existingCategories = allCategories.Intersect(postCategories, new CategoryEqualityComparer());
            var missingCategories = postCategories.Except(allCategories, new CategoryEqualityComparer());

            var categories = existingCategories.Concat(missingCategories).ToList();
            if (!post.IsPublished)
            {
                categories.Add(new Category { Name = unpublished });
            }
            else
            {
                categories.RemoveAll(c => c.Name.Equals(unpublished, StringComparison.OrdinalIgnoreCase));
            }

            var postCategory = categories.Select(c => new PostCategory { Category = c});
            return postCategory;
        }

        private bool IsAdmin() => _contextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
    }
}