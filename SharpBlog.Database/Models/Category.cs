using System.Collections.Generic;

namespace SharpBlog.Database.Models
{
    public class Category : Entity<int>
    {
        public string Name { get; set; }

        public virtual ICollection<PostCategory> PostCategories { get; } = new List<PostCategory>();
    }
}
