using Microsoft.EntityFrameworkCore;
using OnlineCasino.Models;

namespace OnlineCasino.Data
{
    public class CasinoContext : DbContext
    {
        public CasinoContext(DbContextOptions<CasinoContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
    }
}
