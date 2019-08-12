using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class PostCategoryBuilder : IEntityTypeConfiguration<PostCategory>
    {
        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.ToTable("PostCategories");
            builder.HasKey(pt => new { pt.PostId, pt.TagId });

            builder.HasOne(pt => pt.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pt => pt.PostId);
            builder.HasOne(pt => pt.Category)
                .WithMany(t => t.PostCategories)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}
