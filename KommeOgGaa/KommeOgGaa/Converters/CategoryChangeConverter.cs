using System;
using System.Globalization;
using System.Windows.Data;

namespace KommeOgGaa.Converters
{
    class CategoryChangeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            
            Person p = (Person)values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if ((bool)values[i] && IsSameCategory(p.Category, i-1))
                {
                    return System.Windows.Visibility.Visible;
                }
            }

            return System.Windows.Visibility.Collapsed;
        }

        private bool IsSameCategory(string cat, int i)
        {
            switch (i)
            {
                case 0: return cat == "A";
                case 1: return cat == "B";
                case 2: return cat == "C";
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
