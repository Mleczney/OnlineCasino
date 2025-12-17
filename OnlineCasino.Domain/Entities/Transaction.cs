using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Domain.Entities
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        BetPlaced,
        BetWon,
        BetLost
    }

    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Částka musí být mezi 0.01 a 1000000")]
        public decimal Amount { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        [StringLength(200, ErrorMessage = "Popis může mít maximálně 200 znaků")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // vztahy
        public Player? Player { get; set; }
    }
}
