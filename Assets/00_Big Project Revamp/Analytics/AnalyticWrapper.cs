using UnityEngine;

namespace Rush
{
    public static class AnalyticWrapper
    {
        public static void LogEvent(string eventName)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }

        public static void LogEvent(string eventName, string parameter, int value)
        {
            FirebaseAnalytics.LogEvent(
                eventName,
                parameter,
                value);
        }

        public static void LogEvent(string eventName, string parameter, string value)
        {
            FirebaseAnalytics.LogEvent(
                eventName,
                parameter,
                value);
        }
    }
}
