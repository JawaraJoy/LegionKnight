using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class ShopPurchaseTracker : MonoBehaviour
    {
        private const string KeyPurchased = "ShopPurchased_";
        private const string KeyDailyDate = "ShopDaily_";
        private const string DateFormat = "yyyyMMdd";

        // ── Public query ──────────────────────────────────────────────────────

        // Apakah bundle ini saat ini bisa dibeli?
        public bool CanPurchase(ShopBundleConfig bundle)
        {
            return bundle.PurchaseLimit switch
            {
                ShopBundlePurchaseLimit.Unlimited => true,
                ShopBundlePurchaseLimit.OneTime => !HasEverPurchasedInternal(bundle),
                ShopBundlePurchaseLimit.Daily => !HasPurchasedTodayInternal(bundle),
                _ => true
            };
        }

        // Apakah ini pembelian pertama? (untuk first purchase discount)
        public bool IsFirstPurchase(ShopBundleConfig bundle) =>
            !HasEverPurchasedInternal(bundle);

        // Berapa detik lagi bundle daily bisa dibeli kembali (0 jika bukan daily / sudah bisa)
        public double GetDailyResetSecondsRemaining(ShopBundleConfig bundle)
        {
            if (bundle.PurchaseLimit != ShopBundlePurchaseLimit.Daily) return 0;
            if (!HasPurchasedTodayInternal(bundle)) return 0;

            var tomorrow = DateTime.Today.AddDays(1);
            return (tomorrow - DateTime.Now).TotalSeconds;
        }

        // ── Mark ──────────────────────────────────────────────────────────────

        public void MarkPurchased(ShopBundleConfig bundle)
        {
            // Selalu mark ever purchased (untuk first purchase discount)
            UnityService.Instance.SaveData(KeyPurchased + bundle.BaseInfo.Id, true);

            // Untuk daily: simpan juga tanggal hari ini
            if (bundle.PurchaseLimit == ShopBundlePurchaseLimit.Daily)
                UnityService.Instance.SaveData(
                    KeyDailyDate + bundle.BaseInfo.Id,
                    DateTime.Today.ToString(DateFormat));
        }

        // ── Private ───────────────────────────────────────────────────────────

        private bool HasEverPurchasedInternal(ShopBundleConfig bundle)
        {
            string key = KeyPurchased + bundle.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                   && UnityService.Instance.GetData<bool>(key);
        }

        private bool HasPurchasedTodayInternal(ShopBundleConfig bundle)
        {
            string key = KeyDailyDate + bundle.BaseInfo.Id;
            if (!UnityService.Instance.HasData(key)) return false;
            string saved = UnityService.Instance.GetData<string>(key);
            return saved == DateTime.Today.ToString(DateFormat);
        }
    }
}