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
    public class PartyRepository
    {
        //Lijst van alle feestjes ophalen.
        public static async Task<List<Party>> GetPartys()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.PARTIES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Party> data = JsonConvert.DeserializeObject<List<Party>>(json);
                return data;
            }
        }
    }
}
