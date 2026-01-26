
using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ValidationRules;
    public class LoginValidation : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {


        var input = (value ?? "").ToString().Trim();

        if (string.IsNullOrEmpty(input))
            return new ValidationResult(false, "Поле обязательно");

        if (input.Length < 6)
            return new ValidationResult(false, "Минимум 6 символов");

        if (input.Length > 19)
            return new ValidationResult(false, "Максимум 19 символов");

        if (!input.All(c =>
            (c >= 'a' && c <= 'z') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= '0' && c <= '9')))
        {
            return new ValidationResult(false, "Только английские буквы и цифры");
        }

        return ValidationResult.ValidResult;
    }

}
