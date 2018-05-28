using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace KommeOgGaa.Converters
{
    public class ValueToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Laver int/double om til thickness
        /// parameter er hvor mange procent af
        /// værdien skal bruges
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            Thickness result = new Thickness();
            parameter = parameter == null ? "100" : parameter;
            string[] parVal = (parameter as string).Split(' ', ',');
            double[] pro = { 0, 0, 0, 0 };

            switch (parVal.Length)
            {
                case 0:
                    pro[0] = 100;
                    pro[1] = 100;
                    pro[2] = 100;
                    pro[3] = 100;
                    break;

                case 1:
                    pro[0] = System.Convert.ToDouble(parVal[0]);
                    pro[1] = System.Convert.ToDouble(parVal[0]);
                    pro[2] = System.Convert.ToDouble(parVal[0]);
                    pro[3] = System.Convert.ToDouble(parVal[0]);
                    break;
                case 2:
                    pro[0] = System.Convert.ToDouble(parVal[0]);
                    pro[1] = System.Convert.ToDouble(parVal[1]);
                    pro[2] = System.Convert.ToDouble(parVal[0]);
                    pro[3] = System.Convert.ToDouble(parVal[1]);
                    break;
                case 4:
                    pro[0] = System.Convert.ToDouble(parVal[0]);
                    pro[1] = System.Convert.ToDouble(parVal[1]);
                    pro[2] = System.Convert.ToDouble(parVal[2]);
                    pro[3] = System.Convert.ToDouble(parVal[3]);
                    break;
            }


            double[] val = null;
            if (value is double)
            {
                val = new double[] {
                    (double)value,
                    (double)value,
                    (double)value,
                    (double)value
                };
            }
            else if (value is Thickness)
            {

                val = new double[] {
                    ((Thickness)value).Left,
                    ((Thickness)value).Top,
                    ((Thickness)value).Right,
                    ((Thickness)value).Bottom
                };
            }



            result.Left = val[0] * (pro[0] / 100);
            result.Top = val[1] * (pro[1] / 100);
            result.Right = val[2] * (pro[2] / 100);
            result.Bottom = val[3] * (pro[3] / 100);

            return result;
        }

        /// <summary>
        /// Bruges ikke
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
