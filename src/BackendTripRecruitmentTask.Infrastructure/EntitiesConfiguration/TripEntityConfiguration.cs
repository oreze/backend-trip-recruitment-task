using BackendTripRecruitmentTask.Domain;
using BackendTripRecruitmentTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;

public class TripEntityConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasKey(i => i.ID);

        builder.Property(i => i.Name).HasMaxLength(Constants.MaximumTripNameLength).IsRequired();
        builder.Property(i => i.Description);
        builder.Property(i => i.StartDate).IsRequired();
        builder.Property(i => i.NumberOfSeats).IsRequired();

        builder.HasMany(i => i.Registrations)
            .WithOne(i => i.Trip)
            .HasForeignKey(i => i.TripID);

        builder.HasOne(i => i.Country)
            .WithMany()
            .HasForeignKey(i => i.CountryThreeLetterCode)
            .IsRequired();

        builder.HasIndex(i => i.Name)
            .IsUnique();
    }
}