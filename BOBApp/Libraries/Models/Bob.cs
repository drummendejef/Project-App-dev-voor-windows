using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public override string ToString()
        {
            return this.LicensePlate;
        }
    }
}
