using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class CommentMapper : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c=>c.Id);
            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Author)
                .IsRequired();
            builder.Property(c => c.Email);

            builder.HasOne(c=>c.Post)
                .WithMany(p=>p.Comments)
                .HasForeignKey(c=>c.PostId);
        }
    }
}
