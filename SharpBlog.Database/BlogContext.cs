using Microsoft.EntityFrameworkCore;
using SharpBlog.Database.Mappers;
using SharpBlog.Database.Models;
using System.Linq;

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
			
            builder.ApplyConfiguration(new UserMapper());
            builder.ApplyConfiguration(new PostMapper());
            builder.ApplyConfiguration(new CategoryMapper());
            builder.ApplyConfiguration(new CommentMapper());
            builder.ApplyConfiguration(new PostCategoryMapper());
        }

    }
}
