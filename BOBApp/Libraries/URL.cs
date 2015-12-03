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
        public static string USER_REGISTER = BASE + "/user/register";
        public static string USER_EDIT = BASE + "/user/edit";
        public static string TRIPS = BASE + "/trips";
        public static string CURRENTTRIP = BASE + "/trips/current";
        public static string POINTS = BASE + "/user/points";
        public static string TOTALPOINTS = BASE + "/user/points/amount";
        public static string PARTIES = BASE + "/parties";
        public static string MERKEN = BASE + "/autotypes/";
    }
}
