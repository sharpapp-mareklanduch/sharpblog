using System;
using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Core.Models
{
    public class CommentDto
    {
        public int Id { get; set; }

		[Required]
		public int PostId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Author field is required.")]
        public string Author { get; set; }

        [EmailAddress(ErrorMessage = "This is not a valid email address.")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment field is required.")]
        public string Content { get; set; }

        public DateTime InputDate { get; set; }

    }
}