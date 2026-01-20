
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
    private DbService dbService = new();
    public override ValidationResult Validate(object value, CultureInfo
    cultureInfo)
    {
        var input = (value ?? "").ToString().Trim().ToLower();
        if (input == string.Empty)
        {
            return new ValidationResult(false, "Ввод информации в поле обязателен");
        }
        if (input.Length <= 5)
        {
            return new ValidationResult(false, "Должно быть больше пяти символов");
        }
        if (input.Length >= 20)
        {
            return new ValidationResult(false, "Должно быть менее двадцати символов");
        }
        dbService.GetAll();
        foreach (User user in dbService.Users)
        {
            if (user.Username == input)
                return new ValidationResult(false, "Такой пользователь уже существует");
        }
        return ValidationResult.ValidResult;
    }

}
