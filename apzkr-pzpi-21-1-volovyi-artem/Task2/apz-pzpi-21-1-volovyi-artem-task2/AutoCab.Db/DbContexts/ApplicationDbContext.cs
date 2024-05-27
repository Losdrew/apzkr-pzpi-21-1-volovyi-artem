using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AutoCab.Db.Models;

namespace AutoCab.Db.DbContexts;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public new DbSet<User> Users { get; set; }
    public new DbSet<Role> Roles { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>()
            .Property(trip => trip.StartDateTime)
            .HasColumnType("timestamp with time zone");

        modelBuilder.Entity<Trip>()
            .Property(trip => trip.EndDateTime)
            .HasColumnType("timestamp with time zone");

        modelBuilder.HasPostgresEnum<CarStatus>();
        modelBuilder.HasPostgresEnum<TripStatus>();

        modelBuilder.HasPostgresExtension("postgis");

        base.OnModelCreating(modelBuilder);
    }
}