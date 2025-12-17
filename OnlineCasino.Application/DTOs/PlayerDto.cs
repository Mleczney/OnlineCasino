using System.ComponentModel.DataAnnotations;
using OnlineCasino.Application.Validation;

namespace OnlineCasino.Application.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username je povinný")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username musí mít 3-50 znaků")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný")]
        [EmailAddress(ErrorMessage = "Neplatný email")]
        public string Email { get; set; } = string.Empty;

        [MinimumBalance(0)]
        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class RegisterPlayerDto
    {
        [Required(ErrorMessage = "Username je povinný")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username musí mít 3-50 znaků")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný")]
        [EmailAddress(ErrorMessage = "Neplatný email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít minimálně 6 znaků")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrzení hesla je povinné")]
        [Compare("Password", ErrorMessage = "Hesla se neshodují")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Username je povinný")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class DepositDto
    {
        [Required(ErrorMessage = "Částka je povinná")]
        [Range(10, 10000, ErrorMessage = "Částka musí být mezi 10 a 10000 Kč")]
        public decimal Amount { get; set; }
    }
}
