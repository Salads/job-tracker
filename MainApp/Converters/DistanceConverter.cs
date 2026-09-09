using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MainApp.Converters
{
    internal class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int distance = (int)value;
            if(distance == Int32.MaxValue)
            {
                return "?";
            }
            else if(distance == 0)
            {
                return "-";
            }

            return distance;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            if(str == "?")
            {
                return Int32.MaxValue;
            }
            else if (str == "-")
            {
                return 0;
            }

            return Int32.Parse((string)value);
        }
    }
}
