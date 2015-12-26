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
    public class UserRepository
    {
        #region get
        public static async Task<Models.User> GetUser()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.USER);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Models.User data = JsonConvert.DeserializeObject<Models.User>(json);
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


        public static async Task<Models.User.Profile> GetProfile()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var result = client.GetAsync(URL.USER_PROFILE);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    User.Profile data = JsonConvert.DeserializeObject<User.Profile>(json);

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

        public static async Task<Models.Location> GetLocation()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var definition = new { Users_ID = 0, Location = Location.Current, Added = "" };


                    var result = client.GetAsync(URL.USER_LOCATION);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeAnonymousType(json, definition);
                    Location location = data.Location as Location;
                    return location;
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
        public static async Task<Response> Register(Register register)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var newObject = JsonConvert.SerializeObject(register);

                HttpResponseMessage result = await client.PostAsync(URL.USER_REGISTER, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        public static async Task<Response> PostLocation(Location location)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(location);

                    HttpResponseMessage result = await client.PostAsync(URL.USER_LOCATION, new StringContent(newObject, Encoding.UTF8, "application/json"));
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
        public static async Task<Response> EditUser(User.PutUser user)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(user);

                    HttpResponseMessage result = await client.PutAsync(URL.USER_EDIT, new StringContent(newObject, Encoding.UTF8, "application/json"));
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
        public static async Task<Response> ChanteToBob(bool ok)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var definition = new { IsBob = ok };

                    var newObject = JsonConvert.SerializeObject(definition);

                    HttpResponseMessage result = await client.PutAsync(URL.USER_CHANGETOBOB, new StringContent(newObject, Encoding.UTF8, "application/json"));
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






    }
}
