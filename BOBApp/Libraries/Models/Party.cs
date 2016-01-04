using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Libraries.Models
{
    public class Party
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string Organisator { get; set; }
        public int Amount { get; set; }
        public object FacebookEventID { get; set; }

        private string _facebookLink;


        public string FacebookLinkID
        {
            get
            {
                _facebookLink = "https://www.facebook.com/events/" + FacebookEventID + "/";

                return _facebookLink;
            }
            set { _facebookLink = value; }
        }

        public int Cities_ID { get; set; }
        public DateTime Added { get; set; }
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






        public RelayCommand<object> Click { get; set; }

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

    }
}
