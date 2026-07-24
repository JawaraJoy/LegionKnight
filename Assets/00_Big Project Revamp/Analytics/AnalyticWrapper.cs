using UnityEngine;
using Firebase.Analytics;

namespace Rush
{
    public static class AnalyticWrapper
    {

        public static void LogEvent(string eventName)
        {
            FirebaseAnalytics.LogEvent(eventName);
            //write down the others plugins analytic here
        }

        //use for integer
        public static void LogEvent(string eventName, string parameter, int value)
        {
            FirebaseAnalytics.LogEvent(eventName, parameter, value);
            //write down the others plugins analytic here
        }
        // use for decimal
        public static void LogEvent(string eventName, string parameter, double value)
        {
            FirebaseAnalytics.LogEvent(eventName, parameter, value);
            //write down the others plugins analytic here
        }
        public static void LogEvent(string eventName, string parameter, string value)
        {
            FirebaseAnalytics.LogEvent(eventName, parameter, value);
            //write down the others plugins analytic here
        }
    }
}
