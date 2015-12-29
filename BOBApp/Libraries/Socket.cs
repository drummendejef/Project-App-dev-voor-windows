using Libraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries
{
    public class Socket
    {
        public float ID { get; set; }
        public bool Status { get; set; }
        public int From { get; set; }
        public int To { get; set; }

        //optional
        public object Object { get; set; }
        public object Object2 { get; set; }

    }
}
