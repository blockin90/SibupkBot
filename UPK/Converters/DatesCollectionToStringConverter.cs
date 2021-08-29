using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UPK.Converters
{
    class DatesCollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dates = value as IEnumerable<DateTime>;
            if (dates.Count() == 0) {
                throw new ArgumentException("dates collection must not be empty");
            }
            return $"{dates.First().ToShortDateString()} - {dates.Last().ToShortDateString()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}