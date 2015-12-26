using Libraries;
using Libraries.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Repositories
{
    public class AutotypeRepository
    {

        public static async Task<List<Autotype>> GetAutotypes()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.AUTOTYPES);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Autotype> data = JsonConvert.DeserializeObject<List<Autotype>>(json);
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

        public static async Task<Response> PostAutotype(Autotype autotype)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(autotype);

                    HttpResponseMessage result = await client.PostAsync(URL.AUTOTYPES, new StringContent(newObject, Encoding.UTF8, "application/json"));
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
