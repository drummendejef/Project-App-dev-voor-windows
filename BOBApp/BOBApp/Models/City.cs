using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models
{
    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Countries_ID { get; set; }
        public string PostCode { get; set; }

    }
}
