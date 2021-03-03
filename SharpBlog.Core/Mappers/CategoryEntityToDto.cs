using SharpBlog.Core.Dto;

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
