using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Client.ViewModels
{
	public class PostFormViewModel
	{
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		[Required]
		public bool IsPublished { get; set; }
	}
}
