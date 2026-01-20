using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ValidationRules;

public class PasswordValidation : ValidationRule
{

    public override ValidationResult Validate(object value, CultureInfo
    cultureInfo)
    {
        var input = (value ?? "").ToString().Trim();
        if (input == string.Empty)
        {
            return new ValidationResult(false, "Ввод информации в поле обязателен");
        }
        if (input.Length <= 5)
        {
            return new ValidationResult(false, "Должно быть больше пяти символов");
        }
        if (!input.Any(char.IsDigit))
            return new ValidationResult(false, "Пароль должен содержать цифры");
        if (!input.Any(char.IsUpper))
            return new ValidationResult(false, "Пароль должен содержать заглавные буквы");
        if (!input.Any(char.IsLower))
            return new ValidationResult(false, "Пароль должен содержать строчные буквы");
        if (!input.Any(ch => !char.IsLetterOrDigit(ch)))
            return new ValidationResult(false, "Пароль должен содержать специальные символы");
        return ValidationResult.ValidResult;
    }

}
