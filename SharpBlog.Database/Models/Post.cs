using System;
using System.Collections.Generic;

namespace SharpBlog.Database.Models
{
    public class Post : Entity<int>
    {
		public string Title { get; set; }
		public string Content { get; set; }
        public string Author { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? PublicationDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime InputDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PostCategory> PostCategories { get; set; }
    }
}
