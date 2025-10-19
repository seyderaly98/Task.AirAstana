using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task.AirAstana.Domain.Entities;

namespace Task.AirAstana.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code) .IsRequired() .HasMaxLength(256);

        builder.HasIndex(r => r.Code) .IsUnique() .HasDatabaseName("IX_Roles_Code");

        builder.HasMany(r => r.Users) .WithOne(u => u.Role) .HasForeignKey(u => u.RoleId);
    }
}