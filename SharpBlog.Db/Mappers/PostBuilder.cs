using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class PostMapper : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p=>p.Id);
            builder.Property(p=>p.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Author)
                .IsRequired();
            builder.Property(p => p.InputDate)
                .IsRequired();
            builder.Property(p => p.LastModified)
                .IsRequired();
            builder.Property(p => p.IsPublished)
                .IsRequired();

            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);
        }
    }
}
