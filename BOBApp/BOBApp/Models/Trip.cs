using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models
{
    public class Trip
    {
        public int ID { get; set; }
        public string CurrentLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string CityName { get; set; }
        //Timestamp in database, moet misschien nog verandert worden
        public DateTime Added { get; set; }
        public string Name { get; set; }
        
    }
}
