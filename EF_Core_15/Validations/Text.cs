using System.Globalization;
using System.Windows.Controls;
using Microsoft.IdentityModel.Tokens;

namespace EF_Core_15.Validations;

public class Text:ValidationRule
{
    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        var input = (value ?? "").ToString().Trim();
        if (input.Any(c => !char.IsLetter(c))) return new ValidationResult(false, "Не должен содержать цифры");
        return ValidationResult.ValidResult;
    }
}