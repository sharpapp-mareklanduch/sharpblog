using SharpBlog.Core.Models;

namespace SharpBlog.Client.ViewModels
{
	public class PostViewModel
	{
		public PostDto Post { get; set; }
		public CommentFormViewModel CommentFormViewModel { get; set; }

		public PostViewModel(PostDto post)
		{
			Post = post;
			CommentFormViewModel = new CommentFormViewModel { PostId = post.Id };
		}

		public PostViewModel(PostDto post, CommentFormViewModel commentFormViewModel)
		{
			Post = post;
			CommentFormViewModel = commentFormViewModel;
		}
	}
}
