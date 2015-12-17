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
    public class UsersRepository
    {
        #region get
        public static async Task<List<Models.User.All>> GetUsers()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.USERS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.User.All> data = JsonConvert.DeserializeObject<List<Models.User.All>>(json);
                return data;
            }
        }
        public static async Task<Models.User.All> GetUserById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.USERS + "/" + id.ToString());
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.User.All data = JsonConvert.DeserializeObject<Models.User.All>(json);
                return data;
            }
        }
        public static async Task<Models.User.All> GetUserByEmail(string email)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(new { Email = email });

                HttpResponseMessage result = await client.PostAsync(URL.USERS_FIND, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Models.User.All data = JsonConvert.DeserializeObject<Models.User.All>(json);

                return data;
            }

          
        }
        public static async Task<List<Models.User.All>> GetUsersOnline()
        {
            using (HttpClient client = new HttpClient())
            {
           
                var result = client.GetAsync(URL.USERS_ONLINE);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.User.All> data = JsonConvert.DeserializeObject<List<Models.User.All>>(json);

                return data;
            }
        }
        #endregion





    }
}
