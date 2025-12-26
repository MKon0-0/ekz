using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ekz.Pages
{
    public class RoleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                switch (role.ToLower())
                {
                    case "admin": return Brushes.Red;
                    case "manager": return Brushes.Blue;
                    case "driver": return Brushes.Green;
                    case "user": return Brushes.Gray;
                    default: return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}