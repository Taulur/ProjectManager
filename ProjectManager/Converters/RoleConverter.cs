using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectManager.Converters
{
    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string line = value.ToString();

            switch (line)
            {
                case "user":
                    line = "Пользователь";
                    break;
                case "manager":
                    line = "Менеджер";
                    break;
                case "admin":
                    line = "Администратор";
                    break;
                default:
                    line = string.Empty;
                    break;
            }
            return line;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
        {
            return null;
        }
    }
}
