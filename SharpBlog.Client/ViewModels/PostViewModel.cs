using SharpBlog.Core.Models;

namespace SharpBlog.Client.ViewModels
{
	public class PostViewModel
	{
		public Post Post { get; set; }
		public CommentFormViewModel CommentFormViewModel { get; set; }

		public PostViewModel(Post post)
		{
			Post = post;
			CommentFormViewModel = new CommentFormViewModel { PostId = post.Id };
		}

		public PostViewModel(Post post, CommentFormViewModel commentFormViewModel)
		{
			Post = post;
			CommentFormViewModel = commentFormViewModel;
		}
	}
}
