using Libraries;
using Libraries.Models;
using Libraries.Models.relations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Repositories
{
    public class PointRepository
    {
        public static async Task<string> GetTotalPoints()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var definition = new { Points = "" };

                    var result = client.GetAsync(URL.USER_POINTSAMOUNT);
                    string json = await result.Result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeAnonymousType(json, definition);

                    return data.Points;
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


        public static async Task<List<Models.Point>> GetPoints()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.USER_POINTS);
                    string json = await result.Result.Content.ReadAsStringAsync();

                    List<Models.Point> data = JsonConvert.DeserializeObject<List<Models.Point>>(json);
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


        public static async Task<List<Users_PointsDescription>> GetDescription()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = client.GetAsync(URL.USER_POINTS_DESCRIPTION);
                    string json = await result.Result.Content.ReadAsStringAsync();

                    List<Users_PointsDescription> data = JsonConvert.DeserializeObject<List<Users_PointsDescription>>(json);
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
    }
}
