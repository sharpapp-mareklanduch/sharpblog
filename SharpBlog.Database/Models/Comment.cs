using System;

namespace SharpBlog.Database.Models
{
    public class Comment : Entity<int>
    {
        public int PostId { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime InputDate { get; set; }

        public virtual Post Post { get;set; }
    }
}
