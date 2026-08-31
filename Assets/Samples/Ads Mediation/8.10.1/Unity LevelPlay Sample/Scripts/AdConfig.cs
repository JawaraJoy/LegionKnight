public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
#if UNITY_ANDROID
        //return "85460dcd";
        return "239bf6d6d"; // from LevelPlay
#elif UNITY_IOS
        return "8545d445";
#else
        return "unexpected_platform";
#endif
    }

    static string GetBannerAdUnitId()
    {
#if UNITY_ANDROID
        //return "thnfvcsog13bhn08";
        return "Banner_Android_Bidding";
#elif UNITY_IOS
        return "iep3rxsyp9na3rw8";
#else
        return "unexpected_platform";
#endif
    }

    static string GetInterstitialAdUnitId()
    {
#if UNITY_ANDROID
        return "ykhibcrv1rkn2gv4";
#elif UNITY_IOS
        return "wmgt0712uuux8ju4";
#else
        return "unexpected_platform";
#endif
    }

    static string GetRewardedVideoAdUnitId()
    {
#if UNITY_ANDROID
        //return "76yy3nay3ceui2a3";
        return "2idc0jgbu3bxhqvv"; // from LevelPlay
#elif UNITY_IOS
        return "qwouvdrkuwivay5q";
#else
        return "unexpected_platform";
#endif
    }
}