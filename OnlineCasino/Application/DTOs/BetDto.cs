using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Application.DTOs
{
    public class BetDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerUsername { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? WinAmount { get; set; }
        public bool IsWin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
