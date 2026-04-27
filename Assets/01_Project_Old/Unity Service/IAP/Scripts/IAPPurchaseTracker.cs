using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class IAPPurchaseTracker : MonoBehaviour
    {
        private const string KeyPrefix = "IAPPurchased_";
        private const string KeyDailyDate = "IAPDaily_";
        private const string DateFormat = "yyyyMMdd";

        // ── Existing ──────────────────────────────────────────────────────────

        public bool HasPurchased(IAPBundleConfig bundle) =>
            UnityService.Instance.HasData(KeyFor(bundle))
            && UnityService.Instance.GetData<bool>(KeyFor(bundle));

        public void MarkPurchased(IAPBundleConfig bundle)
        {
            UnityService.Instance.SaveData(KeyFor(bundle), true);

            // Save daily date if limit is Daily
            if (bundle.PurchaseLimit == ShopBundlePurchaseLimit.Daily)
                UnityService.Instance.SaveData(
                    KeyDailyFor(bundle),
                    DateTime.Today.ToString(DateFormat));
        }

        // ── New ───────────────────────────────────────────────────────────────

        // Whether player is allowed to purchase right now based on limit type
        public bool CanPurchase(IAPBundleConfig bundle)
        {
            return bundle.PurchaseLimit switch
            {
                ShopBundlePurchaseLimit.Unlimited => true,
                ShopBundlePurchaseLimit.OneTime => !HasPurchased(bundle),
                ShopBundlePurchaseLimit.Daily => !HasPurchasedTodayInternal(bundle),
                _ => true
            };
        }

        // First purchase bonus eligibility — always based on ever purchased,
        // independent of purchase limit type
        public bool IsFirstPurchase(IAPBundleConfig bundle) => !HasPurchased(bundle);

        public double GetDailyResetSecondsRemaining(IAPBundleConfig bundle)
        {
            if (bundle.PurchaseLimit != ShopBundlePurchaseLimit.Daily) return 0;
            if (!HasPurchasedTodayInternal(bundle)) return 0;
            return (DateTime.Today.AddDays(1) - DateTime.Now).TotalSeconds;
        }

        // ── Private ───────────────────────────────────────────────────────────

        private bool HasPurchasedTodayInternal(IAPBundleConfig bundle)
        {
            string key = KeyDailyFor(bundle);
            if (!UnityService.Instance.HasData(key)) return false;
            return UnityService.Instance.GetData<string>(key)
                   == DateTime.Today.ToString(DateFormat);
        }

        private static string KeyFor(IAPBundleConfig bundle) =>
            KeyPrefix + bundle.BaseInfo.Id;

        private static string KeyDailyFor(IAPBundleConfig bundle) =>
            KeyDailyDate + bundle.BaseInfo.Id;
    }
}