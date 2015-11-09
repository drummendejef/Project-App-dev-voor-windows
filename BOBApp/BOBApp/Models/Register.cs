using Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Models
{
    //Zou deze klasse niet beter User heten?
    public class Register
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password =md5.Create(value); }
        }

        public string FacebookID { get; set; }
        public bool IsBob { get; set; }
        public double PricePerKm { get; set; }
        public int BobsType_ID { get; set; }
        public string LicensePlate { get; set; }
        public int AutoType_ID { get; set; }
    }
}
