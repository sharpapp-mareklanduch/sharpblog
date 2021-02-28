using System;

namespace SharpBlog.Common.Models
{
    public class CommentDto
    {
        public int Id { get; set; }
		public int PostId { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime InputDate { get; set; }

    }
}