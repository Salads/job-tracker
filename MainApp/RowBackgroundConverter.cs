using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using MainApp.Models;
using System.Windows.Media;

namespace MainApp
{
    class RowBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result = Brushes.White;
            JobStatus status = (JobStatus)value;

            switch (status)
            {
                case JobStatus.Applied:
                    result = Brushes.Beige;
                    break;
                case JobStatus.Accepted:
                    result = Brushes.LightGreen;
                    break;
                case JobStatus.Interviewing:
                    result = Brushes.LightBlue;
                    break;
                case JobStatus.Rejected:
                    result = Brushes.PaleVioletRed;
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
