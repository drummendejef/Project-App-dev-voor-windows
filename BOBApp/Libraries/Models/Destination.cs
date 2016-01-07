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
        public string Name { get; set; }
    }
}
