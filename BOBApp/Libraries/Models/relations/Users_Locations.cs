using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models.relations
{
    public class Users_Locations
    {
        public int Users_ID { get; set; }
        public string Location { get; set; }
        public DateTime Added { get; set; }
    }
}
