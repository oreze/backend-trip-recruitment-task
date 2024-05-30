using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BackendTripRecruitmentTask.Infrastructure.Data;

public class TripDbContext : DbContext
{
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Trip> Trips { get; set; }
    public virtual DbSet<Registration> Registrations { get; set; }
    public virtual DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CountryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RegistrationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TripEntityConfiguration());
    }
}

