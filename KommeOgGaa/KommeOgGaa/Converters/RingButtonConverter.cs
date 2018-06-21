using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace KommeOgGaa.Converters
{
    class RingButtonConverter : IValueConverter
    {
        public double LevelSpacePercentage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percentageUse = LevelSpacePercentage;
            string[] args = ((string)parameter).Split('|');

            if (args.Contains("Procent"))
            {
                int proID = args.ToList().IndexOf("Procent") + 1;
                percentageUse = System.Convert.ToDouble(args[proID], CultureInfo.InvariantCulture);
            }

            int level = args.Length > 1 ? int.Parse(args[1]) : 1;
            double val = GetPercentageValue(level, (double)value, percentageUse);
            
            if (args.Length > 2 && args[2].ToLower() == "reverse")
            {
                val = (double)value - val;
            }


            if (args.Length > 2 && args[2].ToLower() == "split")
            {
                val = val * System.Convert.ToDouble(args[3], CultureInfo.InvariantCulture);
               // System.Diagnostics.Debug.WriteLine(args[0].ToLower() + " T: " + double.Parse(args[3]));
            }


            //System.Diagnostics.Debug.WriteLine(args[0].ToLower() + " R: " + val + "V: " + value);

            switch (args[0].ToLower())
            {
                case "double": return val;
                case "thickness": return new Thickness(val);
            }

            return value;
        }


        private double GetPercentageValue(int level, double value, double percentage)
        {
            return (level * (value * percentage));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}
