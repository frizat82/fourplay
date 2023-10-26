using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fourplay.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<NFLPicks> NFLPicks { get; set; }
    public DbSet<NFLSpreads> NFLSpreads { get; set; }
    public DbSet<NFLScores> NFLScores { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Adds Quartz.NET SQLite schema to EntityFrameworkCore
        modelBuilder.AddQuartz(builder => builder.UseSqlite());


        modelBuilder.Entity<NFLSpreads>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValue(Guid.NewGuid());
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasKey(x => new { x.Season, x.NFLWeek, x.HomeTeam });

        });
        modelBuilder.Entity<NFLScores>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasKey(x => new { x.Season, x.NFLWeek, x.HomeTeam });
            entity.Property(e => e.Id).HasDefaultValue(Guid.NewGuid());
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<NFLPicks>(entity =>
        {
            entity
                        .HasOne(e => e.User)
                        .WithMany()
                        .HasForeignKey(e => e.UserId)
                        .IsRequired();
            entity.HasKey(e => e.Id);
            entity.HasKey(x => new { x.UserId, x.NFLWeek });
            entity.Property(e => e.Id).HasDefaultValue(Guid.NewGuid());
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
