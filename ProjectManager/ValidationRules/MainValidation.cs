using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ValidationRules;
    public class MainValidation : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo
        cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();
            if (input == string.Empty)
            {
                return new ValidationResult(false, "Ввод информации в поле обязателен");
            }
            if (input.Length <= 1)
            {
                return new ValidationResult(false, "Должно быть больше одного символа");
            }
        if (input.Length >= 99)
        {
            return new ValidationResult(false, "Должно быть менее ста символов");
        }
        return ValidationResult.ValidResult;
        }

    }
