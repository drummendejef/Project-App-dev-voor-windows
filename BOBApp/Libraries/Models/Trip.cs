using Libraries.Models.relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models
{
    public class Trip
    {
        public int ID { get; set; }
        public int Users_ID { get; set; }
        public int Bobs_ID { get; set; }
        public int Party_ID { get; set; }
        public int Destinations_ID { get; set; }
        public string Friends { get; set; }
        public DateTime Added { get; set; }
        public string Status_Name { get; set; }

        public bool Active { get; set; }


        //optional
        public double? Rating { get; set; }
        public int? StatusID { get; set; }

        public class All
        {
            public Trip Trip { get; set; }
            public Party Party { get; set; }
            public Users_Destinations Destination { get; set; }
            public User.All User { get; set; }
            public Bob.All Bob { get; set; }
        }
    }
}
