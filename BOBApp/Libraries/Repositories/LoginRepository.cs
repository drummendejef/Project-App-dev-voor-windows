using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.System;

namespace Libraries.Repositories
{
    public class LoginRepository
    {
      
        public static async Task<Response> Login(string email, string password)
        {
            Response online = await Online();
            if (online.Success == true)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(URL.BASE);

                        var newObject = JsonConvert.SerializeObject(new { Email = email, Password = password });

                        HttpResponseMessage result = await client.PostAsync(URL.AUTH_LOGIN, new StringContent(newObject, Encoding.UTF8, "application/json"));
                        string json = await result.Content.ReadAsStringAsync();
                        Response data = JsonConvert.DeserializeObject<Response>(json);

                        return data;
                    }

                }
                catch(JsonException jex)
                {
                    return new Response() { Error = "Parse Error: " + jex.ToString(), Success = false };
                }
                catch (Exception ex)
                {
                    return new Response() { Error = ex.Message.ToString(), Success = false };
                }
            }
            else
            {
                return online;
            }

        }

        public static async Task<Response> Online()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.ONLINE);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Response data = JsonConvert.DeserializeObject<Response>(json);
                    return data;
                }
            }
            catch (Exception ex)
            {

                return new Response() { Error = "Server Offline", Success = false };
            }



        }

        public static async Task<Boolean> LoginFacebook()
        {
            //Deze Boolean nodig? Voorlopig in commentaar
            //Boolean success = false;

            Uri url = new Uri(URL.AUTH_FACEBOOK);
            Boolean launched = await Launcher.LaunchUriAsync(url);

            return launched;
        }


        public static async Task<Response> LogOff()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(new { });

                    HttpResponseMessage result = await client.PostAsync(URL.AUTH_LOGOFF, new StringContent(newObject, Encoding.UTF8, "application/json"));
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
