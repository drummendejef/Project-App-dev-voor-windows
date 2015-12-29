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
using System.Diagnostics;

namespace Libraries.Repositories
{
    public class TripRepository
    {
        #region get
        //string moet nog verandert worden
        public static async Task<List<Trip>> GetTrips()
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.TRIPS);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Trip> data = JsonConvert.DeserializeObject<List<Trip>>(json);
                    return data;
                }
            }
            catch (JsonException jex)
            {
                Response res = new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
            catch (Exception ex)
            {
                Response res = new Response() { Error = ex.Message.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
        }
        public static async Task<Trip> GetTripByID(int id)
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.TRIPS + "/" + id.ToString());
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Trip data = JsonConvert.DeserializeObject<Trip>(json);
                    return data;
                }
            }
            catch (JsonException jex)
            {
                Response res = new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
            catch (Exception ex)
            {
                Response res = new Response() { Error = ex.Message.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
        }
        //string moet nog verandert worden
        public static async Task<Trip> GetCurrentTrip()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.CURRENTTRIP);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Trip data = JsonConvert.DeserializeObject<Trip>(json);
                    return data;
                }
            }
            catch (JsonException jex)
            {
                Response res = new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
            catch (Exception ex)
            {
                Response res = new Response() { Error = ex.Message.ToString(), Success = false };
                Debug.WriteLine(res);
                return null;
            }
        }
        #endregion

        #region post
        public static async Task<Response> PostTrip(Trip trip)
        {
            try
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
            catch (JsonException jex)
            {
                return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
              
            }
            catch (Exception ex)
            {
                return new Response() { Error = ex.Message.ToString(), Success = false };
               
            }
        }
        public static async Task<Response> Difference(Location from, Location to)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var newObject = JsonConvert.SerializeObject(new { From = from, To = to });

                    HttpResponseMessage result = await client.PostAsync(URL.TRIPS_DIFFERENECE, new StringContent(newObject, Encoding.UTF8, "application/json"));
                    string json = await result.Content.ReadAsStringAsync();
                    Response data = JsonConvert.DeserializeObject<Response>(json);

                    return data;
                }
            }
            catch (JsonException jex)
            {
                return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };

            }
            catch (Exception ex)
            {
                return new Response() { Error = ex.Message.ToString(), Success = false };

            }
        }

        public static async Task<Response> PostLocation(Trips_Locations trip)
        {
            try
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
            catch (JsonException jex)
            {
                return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };

            }
            catch (Exception ex)
            {
                return new Response() { Error = ex.Message.ToString(), Success = false };

            }
        }

        #endregion


        public static async Task<Response> PutActive(int tripsID, bool active)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var newObject = JsonConvert.SerializeObject(new { TripsID = tripsID, Active = active });

                    HttpResponseMessage result = await client.PutAsync(URL.TRIPS_ACTIVE, new StringContent(newObject, Encoding.UTF8, "application/json"));
                    string json = await result.Content.ReadAsStringAsync();
                    Response data = JsonConvert.DeserializeObject<Response>(json);

                    return data;
                }
            }
            catch (JsonException jex)
            {
                return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };

            }
            catch (Exception ex)
            {
                return new Response() { Error = ex.Message.ToString(), Success = false };

            }
        }

    }

}
