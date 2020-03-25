using SharpBlog.Core.Models;
using System.Linq;

namespace SharpBlog.Core.Mappers
{
	public static class CategoryEntityToDto
	{
		public static CategoryDto ToDto(this Database.Models.Category entity)
		{
			return new CategoryDto
			{
				Name = entity.Name
			};
		}
	}
}
