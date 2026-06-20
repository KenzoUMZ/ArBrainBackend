using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Beer> Beers => Set<Beer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beer>(entity =>
        {
            entity.ToTable("beers");

            entity.HasKey(b => b.Id);

            entity.Property(b => b.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(b => b.Style)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(b => b.Abv)
                .HasPrecision(4, 2);

            entity.Property(b => b.Price)
                .HasPrecision(10, 2);

            entity.Property(b => b.CreatedAt)
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasIndex(b => b.Name);
            entity.HasIndex(b => b.IsActive);
        });
    }
}
