using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fourplay.Data;

public class ApplicationDbContext : IdentityDbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }
    public DbSet<LeagueUserMapping> LeagueUserMapping { get; set; }
    public DbSet<NFLPicks> NFLPicks { get; set; }
    public DbSet<NFLSpreads> NFLSpreads { get; set; }
    public DbSet<NFLScores> NFLScores { get; set; }
    public DbSet<LeagueInfo> LeagueInfo { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        // Adds Quartz.NET SQLite schema to EntityFrameworkCore
        modelBuilder.AddQuartz(builder => builder.UseSqlite());

        modelBuilder.Entity<LeagueInfo>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.LeagueName, x.Season }).IsUnique(true);

        });
        modelBuilder.Entity<LeagueUserMapping>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.LeagueId, x.UserId }).IsUnique(true);

        });
        modelBuilder.Entity<NFLSpreads>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.Season, x.NFLWeek, x.HomeTeam }).IsUnique(true);

        });
        modelBuilder.Entity<NFLScores>(entity => {
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.Season, x.NFLWeek, x.HomeTeam }).IsUnique(true);
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<NFLPicks>(entity => {
            entity.HasOne(e => e.User)
                        .WithMany()
                        .HasForeignKey(e => e.UserId)
                        .IsRequired();
            entity.HasOne(e => e.League)
                        .WithMany()
                        .HasForeignKey(e => e.LeagueId)
                        .IsRequired();
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.UserId, x.NFLWeek }).IsUnique(true);
            entity.Property(e => e.DateCreated).HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
