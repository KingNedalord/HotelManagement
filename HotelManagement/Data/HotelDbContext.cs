using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Bookings> Bookings { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Price> Prices { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateTime>()
            .HaveColumnType("timestamp without time zone");

        configurationBuilder.Properties<DateTime?>()
            .HaveColumnType("timestamp without time zone");
    }

    // rewrite when changing anything updatedAt should be updated
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Room>()
            .Property(r => r.RoomType)
            .HasConversion<string>();

        modelBuilder.Entity<Room>()
            .Property(r => r.Status)
            .HasConversion<string>();

        // ── Global Query Filters (Soft Delete) ─────────────────────────
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<Room>()
            .HasQueryFilter(r => !r.IsDeleted);

        modelBuilder.Entity<Bookings>()
            .HasQueryFilter(b => !b.IsDeleted);

        modelBuilder.Entity<Currency>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Price>()
            .HasQueryFilter(p => !p.IsDeleted);

        // ── Default Values ─────────────────────────────────────────────
        modelBuilder.Entity<User>()
            .Property(u => u.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Room>()
            .Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Bookings>()
            .Property(b => b.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Currency>()
            .Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Price>()
            .Property(p => p.IsDeleted)
            .HasDefaultValue(false);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseModel>();
        var now = DateTime.Now;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = now;
                }
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}