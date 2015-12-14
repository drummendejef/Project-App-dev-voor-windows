using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models.relations
{
    public class Users_Destinations
    {
        public int Users_ID { get; set; }
        public int Destinations_ID { get; set; }
        public bool Default { get; set; }
        public DateTime Added { get; set; }
        public string Name { get; set; }


        //optional

        public int Cities_ID { get; set; }


        private object _Location;

        public object Location
        {
            get { return _Location; }
            set {

                try
                {
                    string v = value.ToString();
                    _Location = JsonConvert.DeserializeObject<Location>(v);
                }
                catch (Exception)
                {

                    
                }
              


            }

        }


    }
}
