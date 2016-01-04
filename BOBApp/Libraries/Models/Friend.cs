using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace Libraries.Models
{
    public class Friend
    {
        public int Users_ID { get; set; }
        public int Friends_ID { get; set; }
        public bool Accepted { get; set; }
        public DateTime Added { get; set; }

        public class All
        {

            public User User1 { get; set; }
            public User User2 { get; set; }
            public DateTime Added { get; set; }
            public bool Accepted { get; set; }

            private object _Location;
            public object Location
            {
                get { return _Location; }
                set
                {

                    try
                    {
                        string v = value.ToString();
                        _Location = JsonConvert.DeserializeObject<Location>(v);
                    }
                    catch (Exception)
                    {
                        _Location = value;
                    }



                }

            }

            private Geopoint geolocation;

            public Geopoint GeoLocation
            {
                get
                {
                    try
                    {
                        BasicGeoposition tempbasic = new BasicGeoposition();
                        tempbasic.Latitude = ((Location)this.Location).Latitude;
                        tempbasic.Longitude = ((Location)this.Location).Longitude;
                        geolocation = new Geopoint(tempbasic);
                        return geolocation;
                    }
                    catch (Exception)
                    {

                        return null;
                    }

                }
                set { geolocation = value; }
            }

            public RelayCommand<object> RouteCommand { get; set; }
            public RelayCommand<object> TakeCommand { get; set; }
            public string RouteCommandText { get; set; }

            //for map
            public RelayCommand<object> ShowCommand { get; set; }
            public Visibility VisibleShow { get; set; }
        }
    }
}
