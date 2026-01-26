using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ValidationRules;
    public class FullnameValidation : ValidationRule
    {

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string input = (value as string ?? "").Trim();

        if (string.IsNullOrWhiteSpace(input))
            return new ValidationResult(false, "Поле обязательно");
        if (input.Length > 50)
        {
            return new ValidationResult(false, "Максимум 50 символов");
        }
        var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length < 2)
            return new ValidationResult(false, "Ожидается минимум Фамилия + Имя");

        if (words.Length > 3)
            return new ValidationResult(false, "Не больше 3 частей");

        foreach (var word in words)
        {
            if (string.IsNullOrEmpty(word))
                continue;

            if (word.Length < 2)
                return new ValidationResult(false, "Каждая часть должна быть минимум 2 символа");
            for (int i = 1; i < word.Length; i++)
            {
                char c = word[i];
                if (!IsLowerRussian(c) && c != '-')
                    return new ValidationResult(false, "Разрешены только русские буквы и дефис");
            }
        }

        return ValidationResult.ValidResult;
    }

    private static bool IsUpperRussian(char c) =>
        c >= 'А' && c <= 'Я' || c == 'Ё';

    private static bool IsLowerRussian(char c) =>
        c >= 'а' && c <= 'я' || c == 'ё';
}

