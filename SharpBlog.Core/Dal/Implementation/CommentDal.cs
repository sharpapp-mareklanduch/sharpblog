using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Core.Mappers;
using SharpBlog.Database;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Dal.Implementation
{
	public class CommentDal : ICommentDal
	{
		private readonly BlogContext _dbContext;

		public CommentDal(BlogContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<CommentDto> Add(CommentDto comment)
		{
			var entity = _dbContext.Comments.Add(new Database.Models.Comment
			{
				PostId = comment.PostId,
				Author = comment.Author,
				Email = comment.Email,
				Content = comment.Content,
				InputDate = DateTime.UtcNow
			}).Entity;

			await _dbContext.SaveChangesAsync();

			return entity.ToDto();
		}

		public async Task Delete(int id)
		{
			var entity = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
			if (entity != null)
            {
                entity.IsDeleted = true;
                _dbContext.Update(entity);
                await _dbContext.SaveChangesAsync();
			}
		}
	}
}