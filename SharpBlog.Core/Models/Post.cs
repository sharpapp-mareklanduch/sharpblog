using System;
using System.Collections.Generic;

namespace SharpBlog.Core.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsPublished { get; set; }

		public DateTime? PublicationDate { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime InputDate { get; set; }


        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}