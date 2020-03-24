using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class PostCategoryMapper : IEntityTypeConfiguration<PostCategory>
    {
        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.HasKey(pc => new { pc.PostId, pc.CategoryId });

            builder.HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategory)
                .HasForeignKey(pc => pc.PostId);
            builder.HasOne(pc => pc.Category)
                .WithMany(c => c.PostCategory)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
