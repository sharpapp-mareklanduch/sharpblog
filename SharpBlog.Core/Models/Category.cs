using System.Collections.Generic;

namespace SharpBlog.Core.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();

        public override string ToString()
        {
            return Name;
        }
    }
}