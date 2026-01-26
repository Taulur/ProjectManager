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

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string input = (value as string ?? "").Trim();
        if (string.IsNullOrEmpty(input))
        {
            return new ValidationResult(false, "Пароль обязателен");
        }
        if (input.Length < 6)
        {
            return new ValidationResult(false, "Пароль должен содержать минимум 6 символов");
        }
        if (input.Length > 20)
        {
            return new ValidationResult(false, "Пароль не должен превышать 20 символов");
        }
        foreach (char c in input)
        {
            if (!IsAllowedCharacter(c))
            {
                return new ValidationResult(false,
                    "Пароль содержит только английские буквы и цифры.");
            }
        }
        bool hasDigit = input.Any(char.IsDigit);
        bool hasUpper = input.Any(char.IsUpper);
        bool hasLower = input.Any(char.IsLower);
        if (!hasDigit)
            return new ValidationResult(false, "Пароль должен содержать хотя бы одну цифру");
        if (!hasUpper)
            return new ValidationResult(false, "Пароль должен содержать хотя бы одну заглавную букву");
        if (!hasLower)
            return new ValidationResult(false, "Пароль должен содержать хотя бы одну строчную букву");
        return ValidationResult.ValidResult;
    }

    private static bool IsAllowedCharacter(char c)
    {
        return
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z') ||
            (c >= '0' && c <= '9');
    }

}
