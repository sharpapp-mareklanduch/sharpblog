using Microsoft.EntityFrameworkCore;
using SharpBlog.Database.Mappers;
using SharpBlog.Database.Models;

namespace SharpBlog.Database
{
    public class BlogContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
			
            builder.ApplyConfiguration(new UserBuilder());
            builder.ApplyConfiguration(new PostBuilder());
            builder.ApplyConfiguration(new CategoryBuilder());
            builder.ApplyConfiguration(new CommentBuilder());
            builder.ApplyConfiguration(new PostCategoryBuilder());
        }

    }
}
