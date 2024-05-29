using BackendTripRecruitmentTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendTripRecruitmentTask.Infrastructure.Data;

public class TripDbContext(DbContextOptions<TripDbContext> options) : DbContext(options)
{
    public virtual DbSet<Trip> Trips { get; set; }
    public virtual DbSet<Registration> Registrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

