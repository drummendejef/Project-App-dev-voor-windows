using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Libraries.Models
{
    public class ChatComment
    {
        public int ID { get; set; }
        public int ChatRooms_ID { get; set; }
        public string Comment { get; set; }
        public DateTime Added { get; set; }
        public int Users_ID { get; set; }

        //optional

        public int FromUser_ID { get; set; }
        public HorizontalAlignment Alignment { get; set; }
    }
}
