using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Application.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název hry je povinný")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Název musí mít 2-100 znaků")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Popis může mít maximálně 500 znaků")]
        public string? Description { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Minimální sázka musí být 1-10000")]
        public decimal MinBet { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Maximální sázka musí být 1-100000")]
        public decimal MaxBet { get; set; }

        public bool IsActive { get; set; }
    }

    public class PlaceBetDto
    {
        [Required]
        public int GameId { get; set; }

        [Required(ErrorMessage = "Částka je povinná")]
        [Range(1, 100000, ErrorMessage = "Částka musí být mezi 1 a 100000")]
        public decimal Amount { get; set; }

        [Range(1, 6, ErrorMessage = "Hádej číslo 1-6")]
        public int? Guess { get; set; }
    }
}
