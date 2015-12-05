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
        public static string AUTH_LOGOFF = BASE + "/auth/logoff";

        public static string USER = BASE + "/user";
        public static string USER_POINTS = BASE + "/user/points";
        public static string USER_POINTSAMOUNT = BASE + "/user/points/amount";
        public static string USER_PROFILE = BASE + "/user/profile";
        public static string USER_REGISTER = BASE + "/user/register";
        public static string USER_EDIT = BASE + "/user/edit";
        public static string USER_LOCATION = BASE + "/user/location";
        public static string USER_CHANGETOBOB = BASE + "/user/change";


        public static string TRIPS = BASE + "/trips";
        public static string CURRENTTRIP = BASE + "/trips/current";

        public static string PARTIES = BASE + "/parties";
        public static string PARTIES_AREA = BASE + "/parties/area";

        public static string CITIES = BASE + "/cities";
        public static string COUNTRIES = BASE + "/countries";
        public static string AUTOTYPES = BASE + "/autotypes/";
        public static string FRIENDS = BASE + "/friends/";

        public static string BOBS = BASE + "/bobs/";
        public static string BOBS_ONLINE = BASE + "/bobs/online/";
        public static string BOBS_FIND = BASE + "/bobs/find/";

        public static string CHATROOMS = BASE + "/chatrooms/";

        public static string DESTINATIONS = BASE + "/destinations/";
        public static string DESTINATIONS_DEFAULT = BASE + "/destinations/default/";

        public static string USERS = BASE + "/users/";
        public static string USERS_ONLINE = BASE + "/users/online/";

        public static string STATUSES = BASE + "/statuses/";

    }
}
