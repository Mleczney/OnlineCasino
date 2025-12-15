namespace OnlineCasino.Models
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
        public int PlayerId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // vztahy
        public Player? Player { get; set; }
    }
}
