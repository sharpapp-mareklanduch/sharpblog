using SharpBlog.Common.Models;
using System.Linq;

namespace SharpBlog.Common.Mappers
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
