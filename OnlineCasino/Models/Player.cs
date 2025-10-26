namespace OnlineCasino.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 1000; // startovní kredit 💰
        public bool IsAdmin { get; set; } = false;

        // vztahy
        public ICollection<Bet>? Bets { get; set; }
    }
}
