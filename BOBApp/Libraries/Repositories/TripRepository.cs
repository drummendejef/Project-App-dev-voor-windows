using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Libraries.Models.relations;

namespace Libraries.Repositories
{
    public class TripRepository
    {
        #region get
        //string moet nog verandert worden
        public static async Task<List<Trip>> GetTrips()
        {

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.TRIPS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Trip> data = JsonConvert.DeserializeObject<List<Trip>>(json);
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
        #endregion

        #region post
        public static async Task<Response> PostTrip(Trip trip)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(trip);

                HttpResponseMessage result = await client.PostAsync(URL.TRIPS, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }
        public static async Task<Response> Difference(Location from, Location to)
        {
            using (HttpClient client = new HttpClient())
            {
                var newObject = JsonConvert.SerializeObject(new { From =from, To = to });

                HttpResponseMessage result = await client.PostAsync(URL.TRIPS_DIFFERENECE, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Response> PostLocation(Trips_Locations trip)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(trip);

                HttpResponseMessage result = await client.PostAsync(URL.TRIPS_LOCATION, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        #endregion

        public static async Task<Response> PutActive(int tripsID, bool active)
        {
            using (HttpClient client = new HttpClient())
            {
                var newObject = JsonConvert.SerializeObject(new { TripsID = tripsID, Active = active });

                HttpResponseMessage result = await client.PostAsync(URL.TRIPS_ACTIVE, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

    }

}
