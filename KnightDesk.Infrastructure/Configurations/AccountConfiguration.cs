using KnightDesk.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KnightDesk.Infrastructure.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties Configuration
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Username)
                .HasMaxLength(255);

            builder.Property(x => x.CharacterName)
                .HasMaxLength(100);

            builder.Property(x => x.Password)
                .HasMaxLength(255);

            builder.Property(x => x.IndexCharacter)
                .IsRequired();

            builder.Property(x => x.IsFavorite)
                .HasDefaultValue(false);

            builder.Property(x => x.ServerInfoId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

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

            builder.HasIndex(x => new { x.Username, x.ServerInfoId });

            builder.HasIndex(x => x.IsFavorite);

            builder.HasIndex(x => x.IsDeleted);

            // Relationships
            builder.HasOne(x => x.ServerInfo)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.ServerInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Table Configuration
            builder.ToTable("Accounts");
        }
    }
}
