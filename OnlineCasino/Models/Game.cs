namespace OnlineCasino.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
}
