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
    public class ChatRoomRepository
    {
        #region get
        public static async Task<List<ChatRoom>> GetChatRooms()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.CHATROOMS);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    List<ChatRoom> data = JsonConvert.DeserializeObject<List<ChatRoom>>(json);
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

        public static async Task<ChatRoom.All> GetChatRoom(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.CHATROOMS + "/" + id);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    ChatRoom.All data = JsonConvert.DeserializeObject<ChatRoom.All>(json);
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
        public static async Task<Response> PostChatRoom(int Bobs_ID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);

                    var definition = new { Bobs_ID = Bobs_ID };
                    var newObject = JsonConvert.SerializeObject(definition);

                    HttpResponseMessage result = await client.PostAsync(URL.CHATROOMS, new StringContent(newObject, Encoding.UTF8, "application/json"));
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

        public static async Task<Response> PostChatComment(ChatComment chatComment)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL.BASE);


                    var newObject = JsonConvert.SerializeObject(chatComment);

                    HttpResponseMessage result = await client.PostAsync(URL.CHATROOMS_COMMENT, new StringContent(newObject, Encoding.UTF8, "application/json"));
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
