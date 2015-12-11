using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Libraries.Models
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }


        public static Location Current = null;
       

    }
}
