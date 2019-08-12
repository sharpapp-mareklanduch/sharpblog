using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
    public class CategoryBuilder : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(x=>x.Id);

            builder.Property(x=>x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
