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
    public class TripRepository
    {
        //string moet nog verandert worden
        public static async Task<Trip> GetTrips()
        {

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.TRIPS);
                string json = await result.Result.Content.ReadAsStringAsync();
                Trip data = JsonConvert.DeserializeObject<Trip>(json);
                return data;
            }
        }
        //string moet nog verandert worden
        public static async Task<Trip> GetCurrentTrip()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.CURRENTTRIP);
                string json = await result.Result.Content.ReadAsStringAsync();
                Trip data = JsonConvert.DeserializeObject<Trip>(json);
                return data;
            }
        }
    }
            

          
}
