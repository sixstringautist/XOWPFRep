using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using XOWPF.Model;
namespace XOWPF.View
{
    class Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var par = values[0] as string;
            var field = values[1] as XOField;
            Point p = new Point();
            p.X = float.Parse(par[0].ToString());
            p.Y = float.Parse(par[2].ToString());
            return field[p].ToString().ToUpper();
           
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
