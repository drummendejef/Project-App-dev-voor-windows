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
    public class DestinationRepository
    {
        #region get
        public static async Task<List<Models.relations.Users_Destinations>> GetDestinations()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.DESTINATIONS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.relations.Users_Destinations> data = JsonConvert.DeserializeObject<List<Models.relations.Users_Destinations>>(json);
                return data;
            }
        }
        public static async Task<Models.relations.Users_Destinations> GetDestinationById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.DESTINATIONS + "/" + id.ToString());
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.relations.Users_Destinations data = JsonConvert.DeserializeObject<Models.relations.Users_Destinations>(json);
                return data;
            }
        }

        public static async Task<Models.relations.Users_Destinations> GetDefaultDestination()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.DESTINATIONS_DEFAULT);
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.relations.Users_Destinations data = JsonConvert.DeserializeObject<Models.relations.Users_Destinations>(json);
                return data;
            }
        }

        #endregion


        #region post
        public static async Task<Response> PostDestination(Destination destination)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(destination);

                HttpResponseMessage result = await client.PostAsync(URL.DESTINATIONS, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        #endregion

        #region put

        #endregion





    }
}
