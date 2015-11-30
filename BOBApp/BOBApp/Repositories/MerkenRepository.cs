using BOBApp.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Repositories
{
    public class MerkenRepository
    {
        public static async Task<List<Merk>> GetMerken()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.MERKEN);
                string json = await result.Result.Content.ReadAsStringAsync();
                    List<Merk> data = JsonConvert.DeserializeObject<List<Merk>>(json);
                  return data;
            }
        }
    }
}
