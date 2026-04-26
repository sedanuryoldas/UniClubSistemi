using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<SavedEvent> SavedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SavedEvent için iki anahtarlı yapı (Composite Key)
        modelBuilder.Entity<SavedEvent>()
            .HasKey(se => new { se.UserId, se.EventId });
    }
}