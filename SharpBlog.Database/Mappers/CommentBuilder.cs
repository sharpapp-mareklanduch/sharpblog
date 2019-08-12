using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class CommentBuilder : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.HasKey(x=>x.Id);

            builder.Property(x=>x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(i => i.Author)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(i => i.Email)
                .HasMaxLength(256);

            builder.HasOne(x=>x.Post)
                .WithMany(x=>x.Comments)
                .HasForeignKey(x=>x.PostId);
        }
    }
}
