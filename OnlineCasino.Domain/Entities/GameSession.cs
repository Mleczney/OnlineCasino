using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Domain.Entities
{
    public class GameSession
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int GameId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EndedAt { get; set; }

        public decimal InitialBalance { get; set; }

        public decimal? FinalBalance { get; set; }

        public int TotalBets { get; set; } = 0;

        public decimal TotalWagered { get; set; } = 0;

        public decimal TotalWon { get; set; } = 0;

        // vztahy
        public Player? Player { get; set; }
        public Game? Game { get; set; }
        public ICollection<Bet>? Bets { get; set; }
    }
}
