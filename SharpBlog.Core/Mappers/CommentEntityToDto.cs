using System;
using System.Collections.Generic;
using System.Text;
using SharpBlog.Common.Models;

namespace SharpBlog.Common.Mappers
{
	public static class CommentEntityToDto
	{
		public static CommentDto ToDto(this Database.Models.Comment entity)
		{
			return new CommentDto
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
