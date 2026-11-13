using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace Rush
{
    internal sealed class IAPPaidOrder
    {
        internal readonly Dictionary<IAPBundleConfig, int> Units = new();
        internal string TransactionId { get; private set; }
        internal string Digest { get; private set; }
        private string m_AccountHash;
        internal bool BelongsTo(string playerId) => string.IsNullOrEmpty(m_AccountHash) || m_AccountHash == Hash(playerId);

        internal static string Hash(string value)
        {
            using var sha = SHA256.Create();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "");
        }

        internal bool PreviouslyAcknowledged { get; private set; }

        internal static IAPPaidOrder Read(Order order, IReadOnlyDictionary<string, IAPBundleConfig> catalog)
        {
            string id = order?.Info?.TransactionID;
            var items = order?.CartOrdered?.Items()?.ToArray();
            if (string.IsNullOrWhiteSpace(id) || items == null || items.Length == 0)
                throw new InvalidOperationException("The store returned an incomplete order.");
            var paid = new IAPPaidOrder { TransactionId = id };
            paid.Digest = Hash(id);
            foreach (var item in items)
            {
                string sku = item?.Product?.definition?.id;
                if (sku == null || !catalog.TryGetValue(sku, out var bundle) || item.Quantity < 1)
                    throw new InvalidOperationException("Unrecognized item or quantity in the order.");
                paid.Units.TryGetValue(bundle, out int prior);
                paid.Units[bundle] = checked(prior + item.Quantity);
            }
            if (Application.platform == RuntimePlatform.Android && DefaultStoreHelper.GetDefaultStoreName() == GooglePlay.Name)
            {
                string receipt = order.Info.Receipt;
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), Application.identifier);
                var verified = validator.Validate(receipt).OfType<GooglePlayReceipt>().Single();
                if (items.Length != 1 || verified.purchaseToken != id || verified.packageName != Application.identifier ||
                    verified.purchaseState != GooglePurchaseState.Purchased || verified.productID != items[0].Product.definition.storeSpecificId)
                    throw new InvalidOperationException("The Google Play receipt does not match this order.");
                string payload = (string)JObject.Parse(receipt)["Payload"];
                string signedJson = (string)JObject.Parse(payload)["json"];
                var purchase = JObject.Parse(signedJson);
                paid.m_AccountHash = order.Info.Google?.ObfuscatedAccountId;
                // This SDK constructs Google CartItems with quantity 1 even for multi-unit payments.
                paid.Units[paid.Units.Keys.Single()] = PaidQuantity(purchase);
                paid.PreviouslyAcknowledged = purchase.Value<bool?>("acknowledged") == true;
            }
            return paid;
        }

        internal static int PaidQuantity(JObject purchase)
        {
            var quantity = purchase["quantity"];
            if (quantity == null) return 1;
            if (quantity.Type != JTokenType.Integer || quantity.Value<long>() < 1)
                throw new InvalidOperationException("Invalid quantity in the signed purchase.");
            return checked((int)quantity.Value<long>());
        }
    }
}

