using CapsuleApi.src.Models;
using Microsoft.EntityFrameworkCore;

namespace CapsuleApi.src.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    public DbSet<Capsule> Capsules { get; set; }
    public DbSet<CapsuleFile> Files { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Capsules)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Capsule>()
            .HasMany(c => c.Files)
            .WithOne(f => f.Capsule)
            .HasForeignKey(f => f.CapsuleId);
    }
}
