using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public Transform CreditPurchaseView;

    public UnityEvent<Product> OnSuccess = null;
    public UnityEvent<PurchaseFailureReason> OnFailed = null;

    public void ShowPurchaseView()
    {
        CreditPurchaseView.gameObject.SetActive(true);
    }

    public void Hide()
    {
        CreditPurchaseView.gameObject.SetActive(false);
    }

    public void OnPurchaseSuccess(Product product)
    {
        OnSuccess?.Invoke(product);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        OnFailed?.Invoke(reason);
    }

    public int ProductId2Credits(string productId)
    {
        switch (productId)
        {
            case "com.daybreak.voicetranslator.basicpackage": return 50;
            case "com.daybreak.voicetranslator.smallpackage": return 150;
            default: return 0;
        }
    }
}
