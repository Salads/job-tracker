using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace MainApp
{
    public class URLConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Link";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri url = new Uri((string)value);
            return url;
        }
    }
}
