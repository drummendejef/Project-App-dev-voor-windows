using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models
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
        public DateTime? Added { get; set; }
        public bool Online { get; set; }
        public string Password { get; set; }

        //optional
        public bool IsBob { get; set; }
        public bool CanBeBob { get; set; }
        public string Location { get; set; }

        public class Profile
        {
            public User User { get; set; }
            public Bob Bob { get; set; }
            public Autotype Autotype { get; set; }
        }
        public class All
        {
            public User User { get; set; }
            public Bob Bob { get; set; }
            public string Location { get; set; }
        }

        public class PutUser
        {
            public int? Bobs_ID { get; set; }
            public int Users_ID { get; set; }
            public String Firstname { get; set; }
            public String Lastname { get; set; }
            public String Email { get; set; }
            public String Cellphone { get; set; }
            public String Password { get; set; }
            public int FacebookID { get; set; }
            public Boolean IsBob { get; set; }
            public Double PricePerKm { get; set; }
            public int? BobsType_ID { get; set; }
            public String LicensePlate { get; set; }
            public int? AutoType_ID { get; set; }
        }

        public override string ToString()
        {
            return this.Firstname + " " + this.Lastname;
        }

      
    }
}
