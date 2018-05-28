using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KommeOgGaa.Converters
{
    class BoolToBrushConverter : BoolToValueConverter<System.Windows.Media.Brush> { }
    class BoolToVisibilityConverter : BoolToValueConverter<System.Windows.Visibility> { }
    class BoolToImageConverter : BoolToValueConverter<System.Windows.Controls.Image> { }

    class BoolToValueConverter<T> : IValueConverter
    {

        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
