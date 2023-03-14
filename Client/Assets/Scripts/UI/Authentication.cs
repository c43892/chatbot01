using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;

public class Authentication : MonoBehaviour
{
    public Transform Root;
    public Transform RetryButton;

    public UnityEvent<string> OnSignIn = null;

    public void SignIn()
    {
        Root.gameObject.SetActive(true);
        RetryButton.gameObject.SetActive(false);

        SignInAnonymously((playerId) =>
        {
            Root.gameObject.SetActive(false);
            OnSignIn?.Invoke(playerId);
        }, (error) =>
        {
            RetryButton.gameObject.SetActive(true);
        });
    }

    // Start is called before the first frame update
    async void SignInAnonymously(Action<string> onResponse, Action<string> onError)
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerId = AuthenticationService.Instance.PlayerId;
            onResponse?.Invoke(playerId);
        }
        catch(Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }
}
