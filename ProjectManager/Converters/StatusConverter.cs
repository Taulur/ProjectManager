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
    public class StatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string line = value.ToString();

            switch (line)
            {
                case "Waiting":
                    line = "Ожидание";
                    break;
                case "Active":
                    line = "Выполняется";
                    break;
                case "Completed":
                    line = "Выполнено";
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
