using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BOBApp.Converters
{
    public class OnlineConverter : IValueConverter
    {
        //De hyperlinks die een emailadres bevatten.
        //Als je wil dat die een mail stuurt, zet je daar mailto: voor.
        //Maar dat werkt niet door databinding. 
        //Dus deze converter zet er mailto: voorzetten.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var online = (bool)value;
                if (online == true)
                {
                    return "ONLINE";
                }
                else
                {
                    return "OFFLINE";
                }
            }
            catch (Exception ex)
            {
                return value;
                
            }
        }

        //ConvertBack niet geimplementeerd, oneway binding.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
