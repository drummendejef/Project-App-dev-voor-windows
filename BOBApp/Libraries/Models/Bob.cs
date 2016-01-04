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
    public class Bob
    {
        public int? ID { get; set; }
        public string LicensePlate { get; set; }
        public DateTime? Added { get; set; }
        public bool? Active { get; set; }
        public bool? Offer { get; set; }
        public double PricePerKm { get; set; }
        public int? Autotype_ID { get; set; }


        public int? BobsType_ID { get; set; }
        public class All
        {
            public User User { get; set; }
            public Bob Bob { get; set; }
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

        public override string ToString()
        {
            return this.LicensePlate;
        }
    }
}
