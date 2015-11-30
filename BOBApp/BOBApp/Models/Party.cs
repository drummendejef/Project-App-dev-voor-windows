using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models
{
    public class Party
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Organisator { get; set; }
        public int Amount { get; set; }
        public string FacebookEventID { get; set; }
        public int Cities_ID { get; set; }
        public DateTime Added { get; set; }
        public string Location { get; set; } 
    }
}
