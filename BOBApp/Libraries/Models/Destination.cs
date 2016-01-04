using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Libraries.Models
{
    public class Destination
    {
        public int ID { get; set; }
        public int Cities_ID { get; set; }
        public string Location { get; set; }

        //Omzetten van de Locatiestring naar Geopoint om op de map te plaatsen
        private Geopoint  geolocation;

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
