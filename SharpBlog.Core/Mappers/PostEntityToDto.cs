using SharpBlog.Core.Dto;
using System.Linq;

namespace SharpBlog.Core.Mappers
{
	public static class PostEntityToDto
	{
		public static PostDto ToDto(this Database.Models.Post entity)
		{
			return new PostDto
			{
				Id = entity.Id,
				Author = entity.Author,
				Title = entity.Title,
				Content = entity.Content,
				InputDate = entity.InputDate,
				PublicationDate = entity.PublicationDate,
				IsPublished = entity.IsPublished,
				LastModified = entity.LastModified,
				Comments = entity.Comments?.Where(c => !c.IsDeleted).Select(c => c?.ToDto()),
				Categories = entity.PostCategory?.Select(pc => pc?.Category?.ToDto())
			};
		}
	}
}
