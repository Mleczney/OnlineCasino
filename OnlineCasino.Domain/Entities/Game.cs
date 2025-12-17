using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název hry je povinný")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Název musí mít 2-100 znaků")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Popis může mít maximálně 500 znaků")]
        public string? Description { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Minimální sázka musí být 1-10000")]
        public decimal MinBet { get; set; } = 10;

        [Required]
        [Range(1, 100000, ErrorMessage = "Maximální sázka musí být 1-100000")]
        public decimal MaxBet { get; set; } = 1000;

        public bool IsActive { get; set; } = true;

        // vztahy
        public ICollection<Bet>? Bets { get; set; }
        public ICollection<GameSession>? GameSessions { get; set; }
    }
}
