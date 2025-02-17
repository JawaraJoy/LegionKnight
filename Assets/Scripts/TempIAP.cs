using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class TempIAP : MonoBehaviour
{
    public void OnPurChaseComplete(Product product)
    {
        if (product.definition.id == "coin_2500_temp")
        {
            FindObjectOfType<GameManager>().BuyCoin(2500);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"You cannot buy {product}, because {failureReason}");
    }

}
