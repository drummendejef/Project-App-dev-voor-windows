using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Libraries.Models
{
    public class Bob
    {
        public int? ID { get; set; }
        public string LicensePlate { get; set; }
        public DateTime? Added { get; set; }
        public bool? Active { get; set; }
        public double PricePerKm { get; set; }
        public int? Autotype_ID { get; set; }


        public int? BobsType_ID { get; set; }
        public class All
        {
            public User User { get; set; }
            public Bob Bob { get; set; }
            public string Location { get; set; }

            private Geopoint geolocation;

            public Geopoint GeoLocation
            {
                get
                {
                    try
                    {
                        string[] splittedcoord = Location.Split(',', ':', '}');
                        BasicGeoposition tempbasic = new BasicGeoposition();
                        tempbasic.Latitude = double.Parse(splittedcoord[1].ToString());
                        tempbasic.Longitude = double.Parse(splittedcoord[3].ToString());
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

        public override string ToString()
        {
            return this.LicensePlate;
        }
    }
}
