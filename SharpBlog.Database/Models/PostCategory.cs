namespace SharpBlog.Database.Models
{
    public class PostCategory : IEntity
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        public Post Post { get; set; }
        public Category Category { get; set; }
    }
}
