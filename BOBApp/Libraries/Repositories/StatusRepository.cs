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
    public class StatusRepository
    {
        #region get
        public static async Task<List<Models.Status>> GetStatuses()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.STATUSES);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Models.Status> data = JsonConvert.DeserializeObject<List<Models.Status>>(json);
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
