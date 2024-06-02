using MeetingScheduler.Entities;
using MeetingScheduler.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<UserTimeZone> UserTimeZones { get; set; }
    
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Meeting>();
        builder.Entity<UserTimeZone>();
        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<DateTime>()
            .HaveColumnType("timestamp with time zone");
    }
}