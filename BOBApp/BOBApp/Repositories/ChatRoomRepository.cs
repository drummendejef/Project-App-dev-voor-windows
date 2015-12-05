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
    public class ChatRoomRepository
    {
        #region get
        public static async Task<List<Models.ChatRoom>> GetChatRooms()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.CHATROOMS);
                string json = await result.Result.Content.ReadAsStringAsync();
                List<Models.ChatRoom> data = JsonConvert.DeserializeObject<List<Models.ChatRoom>>(json);
                return data;
            }
        }

        public static async Task<Models.ChatRoom.All> GetChatRoom(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.CHATROOMS + "/" + id);
                string json = await result.Result.Content.ReadAsStringAsync();
                Models.ChatRoom.All data = JsonConvert.DeserializeObject<Models.ChatRoom.All>(json);
                return data;
            }
        }

        #endregion


        #region post
        public static async Task<Response> PostChatRoom(int Bobs_ID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL.BASE);

                var definition = new { Bobs_ID = Bobs_ID};
                var newObject = JsonConvert.SerializeObject(definition);

                HttpResponseMessage result = await client.PostAsync(URL.CHATROOMS, new StringContent(newObject, Encoding.UTF8, "application/json"));
                string json = await result.Content.ReadAsStringAsync();
                Response data = JsonConvert.DeserializeObject<Response>(json);

                return data;
            }
        }

        #endregion

        #region put

        #endregion





    }
}
