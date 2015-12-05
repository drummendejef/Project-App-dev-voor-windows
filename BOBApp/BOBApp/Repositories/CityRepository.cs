using BOBApp.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Repositories
{
    public class CityRepository
    {
        #region get
        public static async Task<List<Models.City>> GetCities()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.CITIES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.City> data = JsonConvert.DeserializeObject<List<Models.City>>(json);
                return data;
            }
        }
       
        #endregion


        #region post
    
        
        #endregion

        #region put
     
        #endregion





    }
}
