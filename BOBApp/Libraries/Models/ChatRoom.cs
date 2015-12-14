using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Libraries.Models
{
    public class ChatRoom
    {
        public int ID { get; set; }
        public int Users_ID { get; set; }
        public int Bobs_ID { get; set; }
        public DateTime Added { get; set; }
        public bool Active{ get; set; }
      

        public class All
        {
            public ChatRoom     ChatRoom { get; set; }
            public List<ChatComment> ChatComments { get; set; }
        }
    }
}
