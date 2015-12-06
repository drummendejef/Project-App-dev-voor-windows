using Libraries;
using Libraries.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.CITIES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<City> data = JsonConvert.DeserializeObject<List<City>>(json);
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
