using com.adjust.sdk;
using UnityEngine;

public static partial class AdjustUtility
{
    public static void TrackRevenue(string adSource, double revenue, string currency)
    {
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(adSource);
        adRevenue.setRevenue(revenue, currency);
        Adjust.trackAdRevenue(adRevenue);
    }

    public static void TrackEvent(string eventName)
    {
        AdjustEvent adjustEvent = new AdjustEvent(eventName);
        Adjust.trackEvent(adjustEvent);
    }
}
