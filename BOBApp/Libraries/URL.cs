using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries
{
    public class URL
    {
        public static string SOCKET = "http://bob-2u15832u.cloudapp.net/";

        public static string BASE = "http://bob-2u15832u.cloudapp.net/api";

        public static string ONLINE = BASE + "/auth/online";

        public static string AUTH_LOGIN = BASE + "/auth/local";
        public static string AUTH_FACEBOOK = BASE + "/auth/facebook";
        public static string AUTH_SUCCESS = BASE + "/auth/success";
        public static string AUTH_LOGOFF = BASE + "/auth/logoff";

        public static string USER = BASE + "/user";
        public static string USER_POINTS_DESCRIPTION = USER + "/points/description";
        public static string USER_POINTS = USER + "/points";
        public static string USER_POINTSAMOUNT = USER + "/points/amount";
        public static string USER_PROFILE = USER + "/profile";
        public static string USER_REGISTER = USER + "/register";
        public static string USER_EDIT = USER + "/edit";
        public static string USER_LOCATION = USER + "/location";
        public static string USER_CHANGETOBOB = USER + "/change";


        public static string TRIPS = BASE + "/trips";
        public static string TRIPS_RATING = TRIPS + "/rating";
        public static string TRIPS_LOCATION = TRIPS + "/location";
        public static string TRIPS_ACTIVE = TRIPS + "/active";
        public static string TRIPS_DIFFERENECE = TRIPS + "/difference";
        public static string CURRENTTRIP = TRIPS + "/current";
      

        public static string PARTIES = BASE + "/parties";
        public static string PARTIES_AREA = BASE + "/parties/area";

        public static string CITIES = BASE + "/cities";
        public static string COUNTRIES = BASE + "/countries";
        public static string AUTOTYPES = BASE + "/autotypes";
        public static string FRIENDS = BASE + "/friends";

        public static string BOBS = BASE + "/bobs";
        public static string BOBS_ACTIVE = BOBS + "/offer";
        public static string BOBS_ONLINE = BOBS + "/online";
        public static string BOBS_FIND = BOBS + "/find";
        public static string BOBS_AVG = BOBS + "/avg";
        public static string BOB_TYPES = BOBS + "/types";

        public static string CHATROOMS = BASE + "/chatrooms";
        public static string CHATROOMS_COMMENT = BASE + "/chatrooms/comment";

        public static string DESTINATIONS = BASE + "/destinations";
        public static string DESTINATIONS_DEFAULT = BASE + "/destinations/default";

        public static string USERS = BASE + "/users";
        public static string USERS_ONLINE = USERS + "/online";
        public static string USERS_FIND = USERS + "/find";

        public static string STATUSES = BASE + "/statuses";

        //URLs om locaties op te halen
        public static string BASISURLTOWNTOCOORD = "http://dev.virtualearth.net/REST/v1/Locations?locality=";
        public static string URLBINGKEY = "&key=";
        public static string BINGKEY = "dOUBDBVwN5QvZ1iHg90c~s2bgtqxiAZX20yceA6JFuw~An9qrmMutNOdQJ0PiF_t7WMqjN4lZBOWQaKrphjthrGdwmqvhjUvX8--_O2kP2K5";

       
    }
}
