using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ValidationRules;
    public class DescValidation : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo
        cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();
            if (input == string.Empty)
            {
                return new ValidationResult(false, "Ввод информации в поле обязателен");
            }
            if (input.Length <= 2)
            {
                return new ValidationResult(false, "Должно быть больше двух символов");
            }
        if (input.Length >= 149)
        {
            return new ValidationResult(false, "Должно быть менее 149 символов");
        }
        return ValidationResult.ValidResult;
        }

    }
