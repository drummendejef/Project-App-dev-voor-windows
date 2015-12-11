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
        public int Cities_ID { get; set; }
        public DateTime Added { get; set; }
        public string Location { get; set; }
        private Geopoint geolocation;

        public Geopoint GeoLocation
        {
            get
            {
                string[] splittedcoord = Location.Split(',', ':', '}');
                BasicGeoposition tempbasic = new BasicGeoposition();
                tempbasic.Latitude = double.Parse(splittedcoord[1].ToString());
                tempbasic.Longitude = double.Parse(splittedcoord[3].ToString());
                geolocation = new Geopoint(tempbasic);
                return geolocation;
            }
            set { geolocation = value; }
        }

    }
}
