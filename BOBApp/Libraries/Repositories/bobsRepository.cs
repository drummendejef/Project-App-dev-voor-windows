
using Libraries;
using Libraries.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.BOBS);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Bob.All> data = JsonConvert.DeserializeObject<List<Bob.All>>(json);
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

        public static async Task<Models.Bob.All> GetBobById(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.BOBS + "/" + id.ToString());
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Models.Bob.All data = JsonConvert.DeserializeObject<Models.Bob.All>(json);
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
        public static async Task<string> GetBobAverageById(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.BOBS_AVG + "/" + id.ToString());
                    string json = await result.Result.Content.ReadAsStringAsync();
                    string data = JsonConvert.DeserializeObject<string>(json);
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

        public static async Task<List<Bob.All>> GetBobsOnline()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var result = client.GetAsync(URL.BOBS_ONLINE);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Bob.All> data = JsonConvert.DeserializeObject<List<Bob.All>>(json);

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
        public static async Task<List<BobsType>> GetTypes()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.BOB_TYPES);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<BobsType> data = JsonConvert.DeserializeObject<List<BobsType>>(json);
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
        public static async Task<List<Bob>> FindBobs(int? rating, DateTime minDate, int BobsType_ID, Location location, int? maxDistance)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var definition = new { Rating = rating, MinDate = minDate, BobsType_ID = BobsType_ID, Location = JsonConvert.SerializeObject(location), MaxDistance = maxDistance };
                    var newObject = JsonConvert.SerializeObject(definition);

                    HttpResponseMessage result = await client.PostAsync(URL.BOBS_FIND, new StringContent(newObject, Encoding.UTF8, "application/json"));
                    string json = await result.Content.ReadAsStringAsync();
                    List<Bob> data = JsonConvert.DeserializeObject<List<Bob>>(json);

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

        public static async Task<Response> SetOffer(bool active, int bobs_ID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var definition = new { Active= active, Bobs_ID= bobs_ID};
                    var newObject = JsonConvert.SerializeObject(definition);

                    HttpResponseMessage result = await client.PutAsync(URL.BOBS_ACTIVE, new StringContent(newObject, Encoding.UTF8, "application/json"));
                    string json = await result.Content.ReadAsStringAsync();
                    Response data = JsonConvert.DeserializeObject<Response>(json);

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



    }
}
