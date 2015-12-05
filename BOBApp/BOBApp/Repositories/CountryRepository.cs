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
    public class CountryRepository
    {
        #region get
        public static async Task<List<Models.Country>> GetCountries()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.COUNTRIES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.Country> data = JsonConvert.DeserializeObject<List<Models.Country>>(json);
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
