using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models.relations
{
    public class Bobs_Parties
    {
        public int Users_ID { get; set; }
        public int Bobs_ID { get; set; }
        public int Parties_ID { get; set; }
        public int Statuses_ID { get; set; }
        public int Destinations_ID { get; set; }
        public DateTime Added { get; set; }
    }
}
