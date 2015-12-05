using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models.relations
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
        public string Location { get; set; }
    }
}
