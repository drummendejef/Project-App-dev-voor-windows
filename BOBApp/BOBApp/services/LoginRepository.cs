using BOBApp.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.services
{
    public class LoginRepository
    {
        const string BASE = "http://bob-2u15832u.cloudapp.net/api";
        const string URL_LOGIN = "http://bob-2u15832u.cloudapp.net/api/user/login";
        const string URL = "http://bob-2u15832u.cloudapp.net/api/user/";
        public static async Task<Boolean> Login(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE);
                var pairs = new List<KeyValuePair<string, string>>();
                pairs.Add(new KeyValuePair<string, string>("Email", email));
                pairs.Add(new KeyValuePair<string, string>("Password", md5.Create(password)));

                var content = new FormUrlEncodedContent(pairs);
           

                HttpResponseMessage result = await client.PostAsync(URL_LOGIN, content);
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data.Success;
            }


        }

        public static async Task<Login> GetUser()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL);
                string json = await result.Result.Content.ReadAsStringAsync();
                Login data = JsonConvert.DeserializeObject<Login>(json);
                return data;
            }
        }
    }
}
