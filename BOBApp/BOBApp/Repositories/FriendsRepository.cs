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
    public class FriendsRepository
    {
        #region get
        public static async Task<List<Models.Friend.All>> GetFriends()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.FRIENDS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.Friend.All> data = JsonConvert.DeserializeObject<List<Models.Friend.All>>(json);
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
