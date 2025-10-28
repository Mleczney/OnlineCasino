namespace OnlineCasino.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<Bet>? Bets { get; set; }
    }

}
