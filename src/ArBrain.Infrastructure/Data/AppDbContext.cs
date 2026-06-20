using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Beer> Beers => Set<Beer>();
    public DbSet<BeerFermentationParameters> BeerFermentationParameters => Set<BeerFermentationParameters>();
    public DbSet<Tank> Tanks => Set<Tank>();
    public DbSet<FermentationRecord> FermentationRecords => Set<FermentationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBeer(modelBuilder);
        ConfigureBeerParameters(modelBuilder);
        ConfigureTank(modelBuilder);
        ConfigureFermentationRecord(modelBuilder);
    }

    private static void ConfigureBeer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beer>(entity =>
        {
            entity.ToTable("beers");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Name).HasMaxLength(120).IsRequired();
            entity.Property(b => b.Style).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasIndex(b => b.Name).IsUnique();
            entity.HasIndex(b => b.IsActive);
        });
    }

    private static void ConfigureBeerParameters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeerFermentationParameters>(entity =>
        {
            entity.ToTable("beer_fermentation_parameters");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.MinTemperature).HasPrecision(5, 2);
            entity.Property(p => p.MaxTemperature).HasPrecision(5, 2);
            entity.Property(p => p.MinPh).HasPrecision(4, 2);
            entity.Property(p => p.MaxPh).HasPrecision(4, 2);
            entity.Property(p => p.MinExtract).HasPrecision(6, 2);
            entity.Property(p => p.MaxExtract).HasPrecision(6, 2);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasOne(p => p.Beer)
                .WithOne(b => b.FermentationParameters)
                .HasForeignKey<BeerFermentationParameters>(p => p.BeerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.BeerId).IsUnique();
        });
    }

    private static void ConfigureTank(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tank>(entity =>
        {
            entity.ToTable("tanks");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name).HasMaxLength(120).IsRequired();
            entity.Property(t => t.CapacityLiters).HasPrecision(10, 2);
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasIndex(t => t.Name).IsUnique();
            entity.HasIndex(t => t.IsActive);
        });
    }

    private static void ConfigureFermentationRecord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FermentationRecord>(entity =>
        {
            entity.ToTable("fermentation_records");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.BatchNumber).HasMaxLength(50).IsRequired();
            entity.Property(r => r.Temperature).HasPrecision(5, 2);
            entity.Property(r => r.Ph).HasPrecision(4, 2);
            entity.Property(r => r.Extract).HasPrecision(6, 2);
            entity.Property(r => r.Observations).HasMaxLength(1000);
            entity.Property(r => r.ComplianceStatus).HasConversion<string>().HasMaxLength(30);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasOne(r => r.Beer)
                .WithMany(b => b.FermentationRecords)
                .HasForeignKey(r => r.BeerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Tank)
                .WithMany(t => t.FermentationRecords)
                .HasForeignKey(r => r.TankId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.BatchNumber);
            entity.HasIndex(r => r.RegisteredAt);
            entity.HasIndex(r => r.ComplianceStatus);
        });
    }
}
