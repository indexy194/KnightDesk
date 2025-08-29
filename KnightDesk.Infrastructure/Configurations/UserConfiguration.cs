using KnightDesk.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KnightDesk.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties Configuration
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Username)
                .HasMaxLength(255);

            builder.Property(x => x.Password)
                .HasMaxLength(255);

            builder.Property(x => x.IPAddress)
                .HasMaxLength(45); // Support for IPv6

            // Common Entity Properties

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(x => x.Username);

            builder.HasIndex(x => x.IPAddress);

            builder.HasIndex(x => x.IsDeleted);

            // Table Configuration
            builder.ToTable("Users");
        }
    }
}
