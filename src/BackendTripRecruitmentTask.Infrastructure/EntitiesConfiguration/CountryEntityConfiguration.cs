using BackendTripRecruitmentTask.Domain;
using BackendTripRecruitmentTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;

public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(i => i.ThreeLetterCode);

        builder.Property(x => x.ThreeLetterCode).HasMaxLength(3);
        builder.Property(i => i.Name).HasMaxLength(Constants.MaximumCountryNameLength).IsRequired();
    }
}