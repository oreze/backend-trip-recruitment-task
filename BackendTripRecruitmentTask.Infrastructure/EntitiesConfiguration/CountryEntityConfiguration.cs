using BackendTripRecruitmentTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;

public class CountryEntityConfiguration: IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(i => i.ThreeLetterCode);

        builder.Property(i => i.Name).HasMaxLength(20).IsRequired();
    }
}