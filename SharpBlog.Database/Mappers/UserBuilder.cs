using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpBlog.Database.Models;

namespace SharpBlog.Database.Mappers
{
	public class UserBuilder : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.IsRequired()
				.ValueGeneratedOnAdd();
			builder.Property(i => i.Name)
				.HasMaxLength(128);
			builder.Property(i => i.Email)
				.HasMaxLength(256);
		}
	}
}
