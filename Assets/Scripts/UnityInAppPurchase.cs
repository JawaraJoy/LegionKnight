using com.adjust.sdk;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UnityInAppPurchase : MonoBehaviour
{
    Dictionary<string, string> datalist = new Dictionary<string, string>();
    public void OnPurchaseComplete(Product product)
    {
        //datalist.Add(AFInAppEvents.PURCHASE, product.definition.id);
        //datalist.Add(AFInAppEvents.CURRENCY, product.definition.payout.subtype);
        //datalist.Add(AFInAppEvents.QUANTITY, product.definition.payout.quantity.ToString());
        //datalist.Add(AFInAppEvents.ADD_PAYMENT_INFO, "SUCCESS");
        if (product.definition.payout.subtype == "Coin")
        {
            AdjustEvent purchaseCoinSuccess = new AdjustEvent("mxsci0");
            purchaseCoinSuccess.setTransactionId(product.definition.id);
            Adjust.trackEvent(purchaseCoinSuccess);
            //AppsFlyerSDK.AppsFlyer.sendEvent("af_CoinPurchasesSuccess", datalist);
            Globalvar.CallAnalytics("Purchase_Coin_Success");
        }
        else if (product.definition.payout.subtype == "Diamond")
        {
            AdjustEvent purchaseDiamondSuccess = new AdjustEvent("kifpy7");
            purchaseDiamondSuccess.setTransactionId(product.definition.id);
            Adjust.trackEvent(purchaseDiamondSuccess);
            //AppsFlyerSDK.AppsFlyer.sendEvent("af_DiamondPurchasesSuccess", datalist);
            Globalvar.CallAnalytics("Purchase_Diamond_Success");
        }

        datalist = new Dictionary<string, string>();
    }
    public void OnPurchaseFailed(Product product,PurchaseFailureReason reason)
    {
        //datalist.Add(AFInAppEvents.PURCHASE, product.definition.id);
        //datalist.Add(AFInAppEvents.CURRENCY, product.definition.payout.subtype);
        //datalist.Add(AFInAppEvents.QUANTITY, product.definition.payout.quantity.ToString());
        //datalist.Add(AFInAppEvents.ADD_PAYMENT_INFO, "FAILED");
        //datalist.Add(AFInAppEvents.DESCRIPTION, reason.ToString());
        if(product.definition.payout.subtype == "Coin")
        {
            AdjustEvent purchaseCoinFailed = new AdjustEvent("mvkpv0");
            purchaseCoinFailed.setTransactionId(product.definition.id);
            purchaseCoinFailed.setCallbackId(reason.ToString());
            Adjust.trackEvent(purchaseCoinFailed);
            //AppsFlyerSDK.AppsFlyer.sendEvent("af_CoinPurchasesFailed", datalist);
            Globalvar.CallAnalytics("Purchase_Coin_Error");
        }
        else if(product.definition.payout.subtype == "Diamond")
        {
            AdjustEvent purchaseDiamondFailed = new AdjustEvent("wggd52");
            purchaseDiamondFailed.setTransactionId(product.definition.id);
            purchaseDiamondFailed.setCallbackId(reason.ToString());
            Adjust.trackEvent(purchaseDiamondFailed);
            //AppsFlyerSDK.AppsFlyer.sendEvent("af_DiamondPurchasesFailed", datalist);
            Globalvar.CallAnalytics("Purchase_Diamond_Error");
        }
        datalist = new Dictionary<string, string>();
    }
}