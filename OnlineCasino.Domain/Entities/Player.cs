using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username je povinný")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username musí mít 3-50 znaků")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný")]
        [EmailAddress(ErrorMessage = "Neplatný email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Balance musí být nezáporný")]
        public decimal Balance { get; set; } = 1000; // startovní kredit 💰

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // vztahy
        public ICollection<Bet>? Bets { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<GameSession>? GameSessions { get; set; }
    }
}
