using System.Globalization;
using System.Windows.Controls;
using Microsoft.IdentityModel.Tokens;

namespace EF_Core_15.Validations;

public class Password:ValidationRule
{
    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        var input = (value ?? "").ToString().Trim();

        if (input.IsNullOrEmpty()) return new ValidationResult(false, "Поле пустое");

        if (input != "1234") return new ValidationResult(false, "Пароль не совпадает");
        
        return ValidationResult.ValidResult;
    }
}