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
    public class FriendsRepository
    {
        #region get
        public static async Task<List<Models.Friend.All>> GetFriends()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.FRIENDS);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<Models.Friend.All> data = JsonConvert.DeserializeObject<List<Models.Friend.All>>(json);
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

        public static async Task<Response> PostFriend(int usersID, int friendsID, bool accepted)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var newObject = JsonConvert.SerializeObject(new { UsersID = usersID, FriendsID = friendsID, Accepted = accepted });

                    HttpResponseMessage result = await client.PostAsync(URL.FRIENDS, new StringContent(newObject, Encoding.UTF8, "application/json"));
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

        #region put

        #endregion






    }
}
