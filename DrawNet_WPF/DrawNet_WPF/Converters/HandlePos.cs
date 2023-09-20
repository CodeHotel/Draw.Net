using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DrawNet_WPF.Resizeables;
namespace DrawNet_WPF.Converters
{
    class HandlePos : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            RelativeSource relativeSource = RelativeSource.TemplatedParent; // Store RelativeSource in a variable for debugging
            System.Diagnostics.Debug.WriteLine($"RelativeSource: {relativeSource}");

            if (values.Length == 5 && values[0] is Tuple<int, int> handleType && values[1] is double parentWidth && values[2] is double parentHeight && values[3] is double handleSize && values[4] is double strokeThickness)
            {
                Thickness k = new Thickness(((1 + handleType.Item1) * parentWidth - handleSize) * 0.5, ((1 + handleType.Item2) * parentHeight - handleSize) * 0.5, 0, 0);
                return k;
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for the HandlePos converter.");
        }
    }
}
