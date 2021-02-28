namespace SharpBlog.Common.Models
{
    public class CategoryDto
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}