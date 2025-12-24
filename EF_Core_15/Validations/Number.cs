using System.Globalization;
using System.Windows.Controls;

namespace EF_Core_15.Validations;

public class Number:ValidationRule
{
    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        var input = (value ?? "").ToString().Trim();
        
        if (!double.TryParse(input, NumberStyles.Any, cultureInfo, out double result))
            return new ValidationResult(false, "Неверный формат числа");

        if (result == 0)
            return new ValidationResult(false, "Число не может быть 0");
        return ValidationResult.ValidResult;
    }
}