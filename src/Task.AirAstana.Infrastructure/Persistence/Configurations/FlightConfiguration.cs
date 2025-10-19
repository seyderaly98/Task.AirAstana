using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task.AirAstana.Domain.Entities;

namespace Task.AirAstana.Infrastructure.Persistence.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Origin).IsRequired().HasMaxLength(256);

        builder.Property(f => f.Destination).IsRequired().HasMaxLength(256);

        builder.Property(f => f.Departure) .IsRequired();

        builder.Property(f => f.Arrival) .IsRequired();

        builder.Property(f => f.Status).IsRequired().HasConversion<int>();

        builder.HasIndex(f => f.Origin) .HasDatabaseName("IX_Flights_Origin");

        builder.HasIndex(f => f.Destination) .HasDatabaseName("IX_Flights_Destination");

        builder.HasIndex(f => f.Arrival).HasDatabaseName("IX_Flights_Arrival");

        builder.Property(f => f.Created) .IsRequired();

        builder.Property(f => f.CreatedBy) .HasMaxLength(256);

        builder.Property(f => f.LastModified);

        builder.Property(f => f.LastModifiedBy).HasMaxLength(256);
    }
}