using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public int Bobs_ID { get; set; }
        public string FacebookID { get; set; }
        public DateTime Added { get; set; }
        public bool Online { get; set; }
        public string Password { get; set; }

        //optional
        public bool IsBob { get; set; }
    }
}
