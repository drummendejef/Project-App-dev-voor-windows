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
    public class PointRepository
    {
        public static async Task<string> GetTotalPoints()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.TOTALPOINTS);
                string json = await result.Result.Content.ReadAsStringAsync();
                string data = JsonConvert.DeserializeObject<string>(json);
                return data;
            }
        }
    }
}
