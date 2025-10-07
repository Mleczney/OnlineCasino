namespace OnlineCasino.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public decimal Amount { get; set; }

        public Player? Player { get; set; }
        public Game? Game { get; set; }
    }
}
