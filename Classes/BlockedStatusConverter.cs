using System;
using System.Globalization;
using System.Windows.Data;

namespace ekz.Pages
{
    public class BlockedStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status == "1" || status.ToUpper() == "Y" ? "Заблокирован" : "Активен";
            }
            return "Активен";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}