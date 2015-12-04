using BOBApp.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Repositories
{
    public class UserRepository
    {
        public static async Task<Response> Register(Register register)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.USER_REGISTER);

                var newObject = JsonConvert.SerializeObject(register);

                HttpResponseMessage result = await client.PostAsync(URL.USER_REGISTER, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Response> EditUser(User user)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.USER_EDIT);

                var newObject = JsonConvert.SerializeObject(user);

                HttpResponseMessage result = await client.PutAsync(URL.USER_EDIT, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Models.User> GetUser()
        {
            //Probleem: Deze haalt het profiel op, maar deze wordt nooit geupdate ( geen idee of deze zelfs ook gepost word) bij EditUser
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.USER);
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.User data = JsonConvert.DeserializeObject<Models.User>(json);
                return data;
            }
        }
    }
}
