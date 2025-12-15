using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Domain.Entities
{
    public class Bet
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Částka musí být mezi 1 a 100000")]
        public decimal Amount { get; set; }

        public decimal? WinAmount { get; set; }

        public bool IsWin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // vztahy
        public Player? Player { get; set; }
        public Game? Game { get; set; }
        public int? GameSessionId { get; set; }
        public GameSession? GameSession { get; set; }
    }
}
