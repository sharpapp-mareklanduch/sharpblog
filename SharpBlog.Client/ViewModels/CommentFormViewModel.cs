using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Client.ViewModels
{
	public class CommentFormViewModel
	{
		[Required]
		public int PostId { get; set; }

		[Required]
		public string Name { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Content { get; set; }
	}
}
