using System;
using System.Collections.Generic;
using System.Text;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Mappers
{
	public static class CommentEntityToDto
	{
		public static Comment EntityToDto(this Database.Models.Comment entity)
		{
			return new Comment
			{
				Id = entity.Id,
				PostId = entity.PostId,
				Author = entity.Author,
				Email = entity.Email,
				Content = entity.Content,
				InputDate = entity.InputDate
			};
		}
	}
}
