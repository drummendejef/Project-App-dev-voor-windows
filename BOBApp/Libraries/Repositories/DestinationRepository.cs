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
using System.Diagnostics;

namespace Libraries.Repositories
{
    public class DestinationRepository
    {
        #region get
        public static async Task<List<Models.relations.Users_Destinations>> GetDestinations()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.DESTINATIONS);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Models.relations.Users_Destinations> data = JsonConvert.DeserializeObject<List<Models.relations.Users_Destinations>>(json);
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
        public static async Task<Models.relations.Users_Destinations> GetDestinationById(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.DESTINATIONS + "/" + id.ToString());
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Models.relations.Users_Destinations data = JsonConvert.DeserializeObject<Models.relations.Users_Destinations>(json);
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

        public static async Task<Models.relations.Users_Destinations> GetDefaultDestination()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.DESTINATIONS_DEFAULT);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Models.relations.Users_Destinations data = JsonConvert.DeserializeObject<Models.relations.Users_Destinations>(json);
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
        public static async Task<Response> PostDestination(Destination destination)
        {
            try
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
            catch (JsonException jex)
            {
                return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
            }
            catch (Exception ex)
            {
                return new Response() { Error = ex.Message.ToString(), Success = false };
            }
        }
        public static async Task<Response> PostDefaultDestination(int destinationsID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(new { DestinationsID = destinationsID });

                    HttpResponseMessage result = await client.PostAsync(URL.DESTINATIONS_DEFAULT, new StringContent(newObject, Encoding.UTF8, "application/json"));
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

        #region put

        #endregion






    }
}
