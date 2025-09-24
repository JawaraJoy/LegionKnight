using UnityEngine;
using AppsFlyerSDK;
using System.Collections.Generic;

namespace LegionKnight
{
    public class AppsflyerManager : Singleton<AppsflyerManager>, IAppsFlyerConversionData
    {
        [Header("Appflyer Settings")]
        [SerializeField] string devKey = "YDyuin9YQRjkNK5fLc3kNA";
        [SerializeField] bool enableDebug = true;

        //MAU tracking variables
        private const string LastLoginDateKey = "last_login_date"; // Key for storing the last login date in PlayerPrefs
        private const string MAUCountKey = "mau_count";           // Key for storing monthly active users count in PlayerPrefs

        //revenue tracking variables 
        private const string TotalRevenueKey = "total_revenue";   // Key for storing total revenue in PlayerPrefs
        private const string DAUCountKey = "dau_count";           // Key for storing daily active users in PlayerPrefs

        protected override void Awake()
        {
            base.Awake();

            //enable debug log while testing
            AppsFlyer.setIsDebug(enableDebug);

            AppsFlyer.initSDK(devKey, null, this);
            AppsFlyer.startSDK();

            //track dau when game start
            TrackDAU();

            //track mau when game start
            TrackMAU();

            //recalculating ARPU every 24 hours
            InvokeRepeating(nameof(TrackARPU), 0f, 86400f);
        }

        //conversion & attribution callbacks
        public void onConversionDataSuccess(string conversionData)
        {
            // Handle the conversion data here (the data is returned as a JSON string)
            Debug.Log("Conversion Data: " + conversionData);

            // Optionally, process the conversion data (parse it into a dictionary if needed)
            // Example: Use JSON parsing if you want to process the data
            var conversionDataDict = JsonUtility.FromJson<Dictionary<string, string>>(conversionData);

            // Example: Use the data for attribution
            if (conversionDataDict.ContainsKey("af_source"))
            {
                string source = conversionDataDict["af_source"];
                string campaign = conversionDataDict.ContainsKey("af_campaign") ? conversionDataDict["af_campaign"] : "Unknown";

                // Log or use the attribution data
                Debug.Log("Install Source: " + source);
                Debug.Log("Campaign: " + campaign);

                // Optionally, track the install event
                TrackInstall(source, campaign);
            }
        }

        //installation track
        public void TrackInstall(string source, string campaign)
        {
            var installEvent = new Dictionary<string, string>
            {
                { "source", source },
                { "campaign", campaign }
            };

            // Send the install event to AppsFlyer
            AppsFlyer.sendEvent("install", installEvent);
        }


        //conversion & attribution failed callbacks
        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFailed", error);
        }

        //attribution success callback
        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        }

        //attribution failed callback
        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailed", error);
        }


        #region Revenue Tracking

        /// <summary>
        /// Track in-app purchase revenue.
        /// </summary>
        /// <param name="stockKeepingUnitID">Stock keeping unit ID in play console</param>
        /// <param name="item">Item purchased (e.g., character, pack)</param>
        /// <param name="price">Price of the item in the purchase</param>
        /// <param name="currency">Currency used (e.g., USD, EUR)</param>
        /// <param name="transactionId">Unique transaction ID for the purchase</param>
        /// <param name="store">store name</param>
        public static void TrackInAppPurchase(string stockKeepingUnitID, string item, double price, string currency, string transactionId, string store)
        {
            var purchaseEvents = new Dictionary<string, string>()
            {
                {"stockKeepingUnit_id", stockKeepingUnitID },
                {"item", item},
                {"price", price.ToString() },
                {"currency", currency },
                {"transaction_id", transactionId },
                {"store", store }
            };

            // Send the purchase event to AppsFlyer
            AppsFlyer.sendEvent("in_app_purchase", purchaseEvents);
            TrackRevenue(price);
        }

        /// <summary>
        /// Track revenue generated from ads.
        /// </summary>
        /// <param name="revenue">Revenue from ad impressions (in USD)</param>
        /// <param name="adNetwork">The ad network used (e.g., Facebook, Google)</param>
        public static void TrackAdRevenue(double revenue, string adNetwork)
        {
            // Track ad revenue in AppsFlyer
            var adEvent = new Dictionary<string, string>
        {
            { "ad_network", adNetwork },
            { "ad_revenue", revenue.ToString("0.00") },
            { "currency", "USD" } // Assuming USD as the currency
        };
            AppsFlyer.sendEvent("ad_revenue", adEvent);

            // Track the revenue for ARPU calculation
            TrackRevenue(revenue);
        }

        /// <summary>
        /// Track total revenue (for ARPU calculation).
        /// </summary>
        /// <param name="amount">Amount to add to the total revenue</param>
        private static void TrackRevenue(double amount)
        {
            // Get the current total revenue from PlayerPrefs
            double totalRevenue = PlayerPrefs.GetFloat(TotalRevenueKey, 0);

            // Add the new revenue to the total
            totalRevenue += amount;

            // Save the updated total revenue back to PlayerPrefs
            PlayerPrefs.SetFloat(TotalRevenueKey, (float)totalRevenue);
            PlayerPrefs.Save();
        }

        #endregion

        #region Active User Tracking (DAU - Daily Active Users)

        /// <summary>
        /// Check if the user is active today and track daily active users (DAU).
        /// </summary>
        public static void TrackDAU()
        {
            // Check if the user is active today based on the last login date
            if (IsUserActiveToday())
            {
                // If the user is active today, increment DAU count
                int dauCount = PlayerPrefs.GetInt(DAUCountKey, 0);
                dauCount++;

                // Save the updated DAU count to PlayerPrefs
                PlayerPrefs.SetInt(DAUCountKey, dauCount);
                PlayerPrefs.Save();

                // Optionally, you can log or track DAU in AppsFlyer or another system here.
                Debug.Log("DAU Count: " + dauCount);
            }
        }

        /// <summary>
        /// Check if the user has logged in today.
        /// </summary>
        /// <returns>True if the user is active today, false otherwise</returns>
        public static bool IsUserActiveToday()
        {
            string lastLoginDate = PlayerPrefs.GetString(LastLoginDateKey, "");

            // Get today's date in string format (e.g., "2025-06-15")
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");

            // If the user hasn't logged in today, consider them active and update the login date
            if (lastLoginDate != today)
            {
                // Update the login date to today's date
                PlayerPrefs.SetString(LastLoginDateKey, today);
                PlayerPrefs.Save();
                return true; // User is active today
            }
            return false; // User has already logged in today
        }

        #endregion

        #region MAU - Monthly Active Users

        /// <summary>
        /// Track Monthly Active Users (MAU).
        /// </summary>
        public static void TrackMAU()
        {
            // Check if the user has logged in this month
            if (IsUserActiveThisMonth())
            {
                // If the user is active this month, increment MAU count
                int mauCount = PlayerPrefs.GetInt(MAUCountKey, 0);
                mauCount++;

                // Save the updated MAU count to PlayerPrefs
                PlayerPrefs.SetInt(MAUCountKey, mauCount);
                PlayerPrefs.Save();

                // Optionally, you can log or track MAU in AppsFlyer or another system here.
                Debug.Log("MAU Count: " + mauCount);
            }
        }

        /// <summary>
        /// Check if the user has logged in this month.
        /// </summary>
        /// <returns>True if the user is active this month, false otherwise</returns>
        public static bool IsUserActiveThisMonth()
        {
            string lastLoginDate = PlayerPrefs.GetString(LastLoginDateKey, "");

            // Get the current month and year (e.g., "2025-06")
            string currentMonth = System.DateTime.Now.ToString("yyyy-MM");

            // Check if the last login was in the same month and year
            if (lastLoginDate != currentMonth)
            {
                // If the user hasn't logged in this month, update the login date
                PlayerPrefs.SetString(LastLoginDateKey, currentMonth);
                PlayerPrefs.Save();
                return true; // User is active this month
            }
            return false; // User has already logged in this month
        }
        #endregion

        #region ARPU Calculation

        /// <summary>
        /// Calculate and track ARPU (Average Revenue Per User).
        /// </summary>
        public static void TrackARPU()
        {
            // Get the total revenue from PlayerPrefs
            double totalRevenue = PlayerPrefs.GetFloat(TotalRevenueKey, 0);

            // Get the number of active users (DAU count in this case)
            int activeUsers = GetActiveUsers(); // Get DAU count or MAU count as per your need

            // Calculate ARPU
            if (activeUsers > 0)
            {
                double arpu = totalRevenue / activeUsers;
                Debug.Log("ARPU: " + arpu);

                // Optionally, send ARPU data to AppsFlyer or your analytics system
                AppsFlyer.sendEvent("arpu", new Dictionary<string, string>
                {
                    { "arpu", arpu.ToString("0.00") }
                });
            }
        }

        /// <summary>
        /// Get the current active users count (Daily Active Users - DAU).
        /// </summary>
        /// <returns>The number of active users (DAU count)</returns>
        public static int GetActiveUsers()
        {
            // Return the DAU count from PlayerPrefs
            return PlayerPrefs.GetInt(DAUCountKey, 0);
        }

        #endregion
    }
}
