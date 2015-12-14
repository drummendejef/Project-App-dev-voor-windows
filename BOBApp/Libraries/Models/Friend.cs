using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models
{
    public class Friend
    {
        public int Users_ID { get; set; }
        public int Friends_ID { get; set; }
        public bool Accepted { get; set; }
        public DateTime Added { get; set; }

        public class All
        {

            public User User1 { get; set; }
            public User User2 { get; set; }
            public DateTime Added { get; set; }
            public bool Accepted { get; set; }
        }
    }
}
