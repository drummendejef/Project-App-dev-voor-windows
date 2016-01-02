using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BOBApp.Converters
{
    public class DateTimeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTime time = (DateTime)value;
                return "Gemaakt op " + time.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {

                return value;
            }
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
