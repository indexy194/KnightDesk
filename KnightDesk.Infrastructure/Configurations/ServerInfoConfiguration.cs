using KnightDesk.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KnightDesk.Infrastructure.Configurations
{
    public class ServerInfoConfiguration : IEntityTypeConfiguration<ServerInfo>
    {
        public void Configure(EntityTypeBuilder<ServerInfo> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties Configuration
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IndexServer)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(100);

            // Common Entity Properties
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(x => x.IndexServer)
                .IsUnique();

            builder.HasIndex(x => x.Name);

            builder.HasIndex(x => x.IsDeleted);

            // Table Configuration
            builder.ToTable("ServerInfos");
        }
    }
}
