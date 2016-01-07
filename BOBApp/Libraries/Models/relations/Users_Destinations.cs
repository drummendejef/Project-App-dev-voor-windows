using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace Libraries.Models.relations
{
    public class Users_Destinations
    {
        public int Users_ID { get; set; }
        public int Destinations_ID { get; set; }
        public bool Default { get; set; }
        public DateTime Added { get; set; }
        public string Name { get; set; }


        //optional

        public int Cities_ID { get; set; }
        public string Cities_Name { get; set; }
        public RelayCommand<object> SetDefault { get; set; }
        public RelayCommand<object> Remove { get; set; }
        public Visibility VisibleDefault { get; set; }
        public MapControl Map { get; set; }


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
