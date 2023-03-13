using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public Transform CreditPurchaseView;

    public void ShowPurchaseView()
    {
        CreditPurchaseView.gameObject.SetActive(true);
    }

    public void OnPurchaseSuccess(Product product)
    {
        Debug.Log("ok: " + product.receipt);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("ok: " + product.receipt + ":" + reason);
    }
}
