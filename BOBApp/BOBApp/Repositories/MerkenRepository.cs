﻿using BOBApp.Models;
using Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Repositories
{
    public class MerkenRepository
    {
        public static async Task<List<Autotype>> GetMerken()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URL.MERKEN);
                string json = await result.Result.Content.ReadAsStringAsync();
                    List<Autotype> data = JsonConvert.DeserializeObject<List<Autotype>>(json);
                  return data;
            }
        }
    }
}
