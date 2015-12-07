using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.System;

namespace Libraries.Repositories
{
    public class LoginRepository
    {
       
        public static async Task<Response> Login(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(new {Email=email,Password =password });
           
                HttpResponseMessage result = await client.PostAsync(URL.AUTH_LOGIN,new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }


        }
        public static async Task<Boolean> LoginFacebook()
        {
            //Deze Boolean nodig? Voorlopig in commentaar
            //Boolean success = false;

            Uri url = new Uri(URL.AUTH_FACEBOOK);
            Boolean launched = await Launcher.LaunchUriAsync(url);

            return launched;
        }


        public static async Task<Response> LogOff()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(new { });

                HttpResponseMessage result = await client.PostAsync(URL.AUTH_LOGOFF, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }


        }

    }
}
