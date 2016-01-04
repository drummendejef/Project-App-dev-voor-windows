using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Libraries.Models;

namespace BOBApp.DataTemplateSelectors
{
    public class MapTemplateSelector : DataTemplate
    {
        public DataTemplate UsersMapTemplate { get; set; }
        public DataTemplate BobsMapTemplate { get; set; }
        public DataTemplate PartiesMapTemplate { get; set; }

        public MapTemplateSelector()
        {
             
        }


        //protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        //{
        //    List<object> items = item as List<object>;

        //    //list party
        //    try
        //    {
        //        List<Party> parties = item as List<Party>;
        //        return PartiesMapTemplate;
        //    }
        //    catch (Exception ex)
        //    {

               
        //    }
        //    try
        //    {
        //        List<User.All> users = item as List<User.All>;
        //        return UsersMapTemplate;
        //    }
        //    catch (Exception ex)
        //    {

                
        //    }



        //    try
        //    {
        //        List<Bob.All> bobs = item as List<Bob.All>;
        //        return BobsMapTemplate;
        //    }
        //    catch (Exception ex)
        //    {

               
        //    }

        //    return PartiesMapTemplate;
        //            //list user.all

        //    //list bob.all
        //}
    }
}
