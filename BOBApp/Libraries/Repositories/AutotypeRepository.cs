using Libraries;
using Libraries.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Repositories
{
    public class AutotypeRepository
    {
        public static async Task<List<Autotype>> GetAutotypes()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.AUTOTYPES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Autotype> data = JsonConvert.DeserializeObject<List<Autotype>>(json);
                return data;
            }
        }
        public static async Task<Response> PostAutotype(Autotype autotype)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(autotype);

                HttpResponseMessage result = await client.PostAsync(URL.AUTOTYPES, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }
    }
}
