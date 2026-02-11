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
    public class PriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string line = value.ToString();

            switch (line)
            {
                case "Low":
                    line = "Приоритет низкий";
                    break;
                case "Medium":
                    line = "Приоритет средний";
                    break;
                case "High":
                    line = "Приоритет высокий";
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
