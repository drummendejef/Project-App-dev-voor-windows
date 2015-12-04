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
                var definition = new { Points = "" };

                var result = client.GetAsync(URL.USER_POINTSAMOUNT);
                string json = await result.Result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeAnonymousType(json, definition);
                return data.Points;
            }
        }

        public static async Task<List<Models.Point>> GetPoints()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.USER_POINTS);
                string json = await result.Result.Content.ReadAsStringAsync();

                List<Models.Point> data = JsonConvert.DeserializeObject<List<Models.Point>>(json);
                return data;
            }
        }
    }
}
