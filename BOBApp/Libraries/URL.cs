using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries
{
    public class URL
    {
        public static string BASE = "http://bob-2u15832u.cloudapp.net/api";
        public static string AUTH_LOGIN = BASE + "/auth/local";
        public static string AUTH_FACEBOOK = BASE + "/auth/facebook";
        public static string AUTH_SUCCESS = BASE + "/auth/success";
        public static string USER = BASE + "/user";
        public static string PROFILE = BASE + "/user/profile";
        public static string REGISTER = BASE + "/user/register";
    }
}
