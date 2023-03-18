using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public Transform CreditPurchaseView;

    public UnityEvent<Product> OnSuccess = null;
    public UnityEvent<PurchaseFailureReason> OnFailed = null;

    void Start()
    {
        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add products that will be purchasable and indicate its type.
        builder.AddProduct("com.daybreak.voicetranslator.basicpackage_01", ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

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
            case "com.daybreak.voicetranslator.basicpackage_01": return 50;
            default: return 0;
        }
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("===== OnInitializeFailed 1: " + error.ToString());
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("===== OnInitializeFailed 2: " + error.ToString() + " : " + message);
    }

    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
    {
        return PurchaseProcessingResult.Complete;
    }

    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("===== OnIapFailed: " + product.definition.id + ":" + failureReason.ToString());
    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("===== OnInitialize Succeed");
    }
}
