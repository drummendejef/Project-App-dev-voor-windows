using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Repositories
{
    public class PartyRepository
    {
        //Lijst van alle feestjes ophalen.
        public static async Task<List<Party>> GetParties()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.PARTIES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Party> data = JsonConvert.DeserializeObject<List<Party>>(json);
                return data;
            }
        }

        public static async Task<List<Party>> GetPartiesInArea(string location, double distance)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.PARTIES_AREA);

                var newObject = JsonConvert.SerializeObject(new { Location = location, Distance = distance });

                HttpResponseMessage result = await client.PostAsync(URL.PARTIES_AREA, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                List<Party> data = JsonConvert.DeserializeObject<List<Party>>(json);

                return data;
            }
        }
        public static async Task<Response> PostParty(Party party)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(party);

                HttpResponseMessage result = await client.PostAsync(URL.PARTIES, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }
    }
}
