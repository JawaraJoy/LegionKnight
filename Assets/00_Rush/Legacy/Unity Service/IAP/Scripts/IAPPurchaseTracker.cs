using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class IAPPurchaseTracker : MonoBehaviour
    {
        private string m_Account;
        internal void BindAccount(string account)
        {
            m_Account = account;
            var save = UnityService.Instance.LocalSave;
            if (!save.HasData("IAPLegacyOwner"))
                save.ExecuteTransaction(() => save.SaveData("IAPLegacyOwner", account));
        }

        private const string KeyPrefix = "IAPPurchased_";
        private const string KeyDailyDate = "IAPDaily_";
        private const string DateFormat = "yyyyMMdd";

        // ── Existing ──────────────────────────────────────────────────────────

        public bool HasPurchased(IAPBundleConfig bundle) =>
            UnityService.Instance.LocalSave.GetDataValue<bool>(KeyFor(bundle)) ||
            (!string.IsNullOrEmpty(m_Account) &&
             UnityService.Instance.LocalSave.GetDataValue<string>("IAPLegacyOwner") == m_Account &&
             UnityService.Instance.LocalSave.GetDataValue<bool>(KeyPrefix + bundle.ProductId));

        public void MarkPurchased(IAPBundleConfig bundle)
        {
            UnityService.Instance.LocalSave.SaveData(KeyFor(bundle), true);

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

        private string KeyFor(IAPBundleConfig bundle) =>
            KeyPrefix + (string.IsNullOrEmpty(m_Account) ? "" : m_Account + "_") + bundle.ProductId;

        private static string KeyDailyFor(IAPBundleConfig bundle) =>
            KeyDailyDate + bundle.BaseInfo.Id;
    }
}
