using AppDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organizer> Organizers => Set<Organizer>();
    public DbSet<Festival> Festivals => Set<Festival>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<Presentation> Presentations => Set<Presentation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Festival>()
            .HasOne(f => f.Organizer)
            .WithMany(o => o.Festivals)
            .HasForeignKey(f => f.OrganizerId);

        modelBuilder.Entity<Presentation>()
            .HasOne(p => p.Speaker)
            .WithMany(s => s.Presentations)
            .HasForeignKey(p => p.SpeakerId);
    }
}
