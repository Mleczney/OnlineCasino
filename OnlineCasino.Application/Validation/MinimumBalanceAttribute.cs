using System.ComponentModel.DataAnnotations;

namespace OnlineCasino.Application.Validation
{
    /// <summary>
    /// Custom validation attribute to ensure balance is above minimum required amount
    /// </summary>
    public class MinimumBalanceAttribute : ValidationAttribute
    {
        private readonly decimal _minimumBalance;

        public MinimumBalanceAttribute(double minimumBalance)
        {
            _minimumBalance = (decimal)minimumBalance;
            ErrorMessage = ErrorMessage ?? $"Balance musí být minimálně {_minimumBalance} Kč";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Balance je povinný");
            }

            if (value is decimal balance)
            {
                if (balance < _minimumBalance)
                {
                    return new ValidationResult(ErrorMessage ?? $"Balance musí být minimálně {_minimumBalance} Kč");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Balance musí být číslo");
        }
    }
}
