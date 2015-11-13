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
    public class RegisterRepository
    {
        public static async Task<Response> Register(Register register)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(new { Register = JsonConvert.SerializeObject(register) });

                HttpResponseMessage result = await client.PostAsync(URL.REGISTER, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Register> GetUser(String email)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.PROFILE);
                string json = await result.Result.Content.ReadAsStringAsync();
                Register data = JsonConvert.DeserializeObject<Register>(json);
                return data;
            }
        }
    }
}
