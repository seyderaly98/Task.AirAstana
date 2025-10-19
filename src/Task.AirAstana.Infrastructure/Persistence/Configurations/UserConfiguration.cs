using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task.AirAstana.Domain.Entities;

namespace Task.AirAstana.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username) .IsRequired() .HasMaxLength(256);

        builder.Property(u => u.PasswordHash) .IsRequired() .HasMaxLength(256);

        builder.Property(u => u.RoleId) .IsRequired();

        builder.HasIndex(u => u.Username) .IsUnique() .HasDatabaseName("IX_Users_Username");

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.Created) .IsRequired();

        builder.Property(u => u.CreatedBy).HasMaxLength(256);

        builder.Property(u => u.LastModified);

        builder.Property(u => u.LastModifiedBy) .HasMaxLength(256);
    }
}