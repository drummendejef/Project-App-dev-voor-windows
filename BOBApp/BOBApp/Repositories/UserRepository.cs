using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOBApp.Models;
using System.Net.Http;
using Libraries;
using Newtonsoft.Json;

namespace BOBApp.Repositories
{
    public class UserRepository
    {
        public static async Task<Register> GetUser(string email)
        {
            using (HttpClient client = new HttpClient())
            {
                //Haal de user op via het email adres van deze user
                return null;
            }
        }
    }
}
