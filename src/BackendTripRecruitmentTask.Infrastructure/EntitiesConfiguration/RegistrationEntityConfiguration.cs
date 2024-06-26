using BackendTripRecruitmentTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;

public class RegistrationEntityConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.HasKey(i => i.ID);

        builder.Property(i => i.Email).IsRequired();
        builder.Property(i => i.RegisteredAt).IsRequired();

        builder.HasOne(i => i.Trip)
            .WithMany()
            .HasForeignKey(i => i.TripID)
            .IsRequired();
    }
}