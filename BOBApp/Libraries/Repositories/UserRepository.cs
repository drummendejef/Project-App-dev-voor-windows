using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Repositories
{
    public class UserRepository
    {
        #region get
        public static async Task<Models.User> GetUser()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.USER);
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.User data = JsonConvert.DeserializeObject<Models.User>(json);
                return data;
            }
        }
        public static async Task<Models.User.Profile> GetProfile()
        {
            using (HttpClient client = new HttpClient())
            {

                var result = client.GetAsync(URL.USER_PROFILE);
                string json = await result.Result.Content.ReadAsStringAsync();
                User.Profile data = JsonConvert.DeserializeObject<User.Profile>(json);

                return data;
            }
        }

        public static async Task<Models.Location> GetLocation()
        {
            using (HttpClient client = new HttpClient())
            {
                var definition = new { Users_ID = 0, Location = Location.Current, Added = "" };


                var result = client.GetAsync(URL.USER_LOCATION);
                string json = await result.Result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeAnonymousType(json, definition);
                Location location = data.Location as Location;
                return location;
            }
        }
        #endregion


        #region post
        public static async Task<Response> Register(Register register)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(register);

                HttpResponseMessage result = await client.PostAsync(URL.USER_REGISTER, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Response> PostLocation(Location location)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(location);

                HttpResponseMessage result = await client.PostAsync(URL.USER_LOCATION, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        #endregion

        #region put
        public static async Task<Response> EditUser(User user)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(user);

                HttpResponseMessage result = await client.PutAsync(URL.USER_EDIT, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }
        public static async Task<Response> ChanteToBob(bool ok)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var definition = new { IsBob=ok };

                var newObject = JsonConvert.SerializeObject(definition);

                HttpResponseMessage result = await client.PutAsync(URL.USER_CHANGETOBOB, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }
        #endregion





    }
}
