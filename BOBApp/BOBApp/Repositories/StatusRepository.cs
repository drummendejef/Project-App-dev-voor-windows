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
    public class StatusRepository
    {
        #region get
        public static async Task<List<Models.Status>> GetStatuses()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.STATUSES);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.Status> data = JsonConvert.DeserializeObject<List<Models.Status>>(json);
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
