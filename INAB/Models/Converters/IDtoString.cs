using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace INAB.Models.Converters
{
    public class IDtoString : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            var _account = value as Account;
            return _account.ID.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            return int.Parse(value as string);
        }
    }
}
