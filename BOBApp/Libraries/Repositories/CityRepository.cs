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
    public class CityRepository
    {
        #region get
        public static async Task<List<City>> GetCities()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.CITIES);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<City> data = JsonConvert.DeserializeObject<List<City>>(json);
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
    
        
        #endregion

        #region put
     
        #endregion






    }
}
