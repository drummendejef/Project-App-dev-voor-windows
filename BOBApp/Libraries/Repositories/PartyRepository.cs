using Libraries.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Libraries.Repositories
{
    public class PartyRepository
    {
        //Lijst van alle feestjes ophalen.
        public static async Task<List<Party>> GetParties()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.PARTIES);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Party> data = JsonConvert.DeserializeObject<List<Party>>(json);
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


        public static async Task<Party> GetPartyById(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.PARTIES + '/' + id.ToString());
                    string json = await result.Result.Content.ReadAsStringAsync();
                    Party data = JsonConvert.DeserializeObject<Party>(json);
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


        public static async Task<List<Party>> GetPartiesInArea(string location, double distance)
        {
            try
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
        #region post
        public static async Task<Response> PostParty(Party party)
        {
            try
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
