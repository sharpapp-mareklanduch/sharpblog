using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class PostBuilder : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(x=>x.Id);

            builder.Property(x=>x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(x => x.Author)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(x => x.InputDate)
                .IsRequired();
            builder.Property(x => x.LastModified)
                .IsRequired();
            builder.Property(x => x.IsPublished)
                .IsRequired();

            builder.HasMany(x=>x.Comments).WithOne(x=>x.Post).HasForeignKey(x=>x.PostId);
        }
    }
}
