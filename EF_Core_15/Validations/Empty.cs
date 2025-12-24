using System.Globalization;
using System.Windows.Controls;
using Microsoft.IdentityModel.Tokens;

namespace EF_Core_15.Validations;

public class Empty: ValidationRule
{
    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        var input = (value ?? "").ToString().Trim();

        if (input.IsNullOrEmpty()) return new ValidationResult(false, "Поле пустое");
        
        return ValidationResult.ValidResult;
    }
}