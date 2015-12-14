
using Libraries;
using Libraries.Models;
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
    public class BobsRepository
    {
        #region get
        public static async Task<List<Bob.All>> GetBobs()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.BOBS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Bob.All> data = JsonConvert.DeserializeObject<List<Bob.All>>(json);
                return data;
            }
        }
        public static async Task<Models.Bob.All> GetBobById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.BOBS + "/" + id.ToString());
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.Bob.All data = JsonConvert.DeserializeObject<Models.Bob.All>(json);
                return data;
            }
        }
        public static async Task<List<Bob.All>> GetBobsOnline()
        {
            using (HttpClient client = new HttpClient())
            {

                var result = client.GetAsync(URL.BOBS_ONLINE);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Bob.All> data = JsonConvert.DeserializeObject<List<Bob.All>>(json);

                return data;
            }
        }
        public static async Task<List<BobsType>> GetTypes()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.BOB_TYPES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<BobsType> data = JsonConvert.DeserializeObject<List<BobsType>>(json);
                return data;
            }
        }
        #endregion



        #region post
        public static async Task<List<Bob>> FindBobs(int? rating, DateTime minDate, int BobsType_ID, Location location, int? maxDistance)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var definition = new { Rating = rating, MinDate= minDate, BobsType_ID= BobsType_ID, Location= JsonConvert.SerializeObject(location), MaxDistance=maxDistance };
                var newObject = JsonConvert.SerializeObject(definition);

                HttpResponseMessage result = await client.PostAsync(URL.BOBS_FIND, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                List<Bob> data = JsonConvert.DeserializeObject<List<Bob>>(json);

                return data;
            }
        }

        #endregion



    }
}
