using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite;
using fourplay.Models.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fourplay.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<LeagueUsers> LeagueUsers { get; set; }
    public DbSet<LeagueJuiceMapping> LeagueJuiceMapping { get; set; }
    public DbSet<LeagueUserMapping> LeagueUserMapping { get; set; }
    public DbSet<NFLPicks> NFLPicks { get; set; }
    public DbSet<NFLPostSeasonPicks> NFLPostSeasonPicks { get; set; }
    public DbSet<NFLSpreads> NFLSpreads { get; set; }
    public DbSet<NFLScores> NFLScores { get; set; }
    public DbSet<LeagueInfo> LeagueInfo { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        // Adds Quartz.NET SQLite schema to EntityFrameworkCore
        //modelBuilder.AddQuartz(builder => builder.UseSqlite());
        modelBuilder.AddQuartz(builder => builder.UsePostgreSql());

        modelBuilder.Entity<LeagueUsers>(entity => {
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.GoogleEmail }).IsUnique(true);

        });
        modelBuilder.Entity<LeagueInfo>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.LeagueName }).IsUnique(true);
        });
        modelBuilder.Entity<LeagueUserMapping>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(e => e.League)
                       .WithMany()
                       .HasForeignKey(e => e.LeagueId)
                       .IsRequired();
            entity.HasIndex(x => new { x.LeagueId, x.UserId }).IsUnique(true);
        });
        modelBuilder.Entity<LeagueJuiceMapping>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WeeklyCost).HasDefaultValue(5);
            entity.Property(e => e.JuiceDivisonal).HasDefaultValue(10);
            entity.Property(e => e.JuiceConference).HasDefaultValue(6);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity
                 .Property(b => b.WeeklyCost)
                 .HasDefaultValue(5);
            entity.HasOne(e => e.League)
                          .WithMany()
                          .HasForeignKey(e => e.LeagueId)
                          .IsRequired();
            entity.HasIndex(x => new { x.LeagueId, x.Season }).IsUnique(true);
        });
        modelBuilder.Entity<NFLSpreads>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => new { x.Season, x.NFLWeek, x.HomeTeam }).IsUnique(true);

        });
        modelBuilder.Entity<NFLScores>(entity => {
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.Season, x.NFLWeek, x.HomeTeam }).IsUnique(true);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
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
            entity.HasIndex(x => new { x.UserId, x.LeagueId, x.NFLWeek, x.Season, x.Team }).IsUnique(true);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<NFLPostSeasonPicks>(entity => {
            entity.HasOne(e => e.User)
                        .WithMany()
                        .HasForeignKey(e => e.UserId)
                        .IsRequired();
            entity.HasOne(e => e.League)
                        .WithMany()
                        .HasForeignKey(e => e.LeagueId)
                        .IsRequired();
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.UserId, x.LeagueId, x.NFLWeek, x.Season, x.Team, x.Pick }).IsUnique(true);
            entity.Property(e => e.DateCreated).HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
