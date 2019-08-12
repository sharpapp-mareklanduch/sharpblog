using SharpBlog.Core.Models;
using System.Linq;

namespace SharpBlog.Core.Mappers
{
	public static class PostEntityToDto
	{
		public static Post EntityToDto(this Database.Models.Post entity)
		{
			return new Post
			{
				Id = entity.Id,
				Author = entity.Author,
				Title = entity.Title,
				Content = entity.Content,
				InputDate = entity.InputDate,
				PublicationDate = entity.PublicationDate,
				IsPublished = entity.IsPublished,
				LastModified = entity.LastModified,
				Comments = entity.Comments?.Where(c => !c.IsDeleted).Select(c => c?.EntityToDto())?.ToList()
			};
		}
	}
}
