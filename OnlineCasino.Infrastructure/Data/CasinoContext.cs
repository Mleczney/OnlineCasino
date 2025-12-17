using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Infrastructure.Data
{
    public class CasinoContext : IdentityDbContext<IdentityUser>
    {
        public CasinoContext(DbContextOptions<CasinoContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Player entity
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Game entity
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.MinBet).HasPrecision(18, 2);
                entity.Property(e => e.MaxBet).HasPrecision(18, 2);
            });

            // Configure Bet entity
            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.WinAmount).HasPrecision(18, 2);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.Bets)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.GameSession)
                    .WithMany(gs => gs.Bets)
                    .HasForeignKey(e => e.GameSessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.BalanceBefore).HasPrecision(18, 2);
                entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
                entity.Property(e => e.Description).HasMaxLength(200);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure GameSession entity
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InitialBalance).HasPrecision(18, 2);
                entity.Property(e => e.FinalBalance).HasPrecision(18, 2);
                entity.Property(e => e.TotalWagered).HasPrecision(18, 2);
                entity.Property(e => e.TotalWon).HasPrecision(18, 2);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.GameSessions)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Game)
                    .WithMany(g => g.GameSessions)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
