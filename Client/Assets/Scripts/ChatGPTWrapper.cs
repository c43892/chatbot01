using BrainCloud;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatGPTWrapper : MonoBehaviour
{
    const string END_POINT = "https://api.openai.com/v1/completions";
    const string API_KEY = "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";
    const string MODEL = "text-davinci-003";
    const int MAX_PAYLOAD = 2048;
    const int BAD_REQUEST = 400;

    readonly List<string> converstaionHistory = new();

    public InputField OutputField;
    public InputField InputField;
    public ScrollRect ScollRect;

    const string BRAIN_CLOUD_SERVICE_URL = "https://portal.braincloudservers.com";
    const string BRAIN_CLOUD_APP_ID = "14407";
    const string BRAIN_CLOUD_APP_SECRET = "84b17cdb-c2b1-4304-b401-6b1d7d4e4165";
    
    private BrainCloudWrapper BCW = null;


    public void Start()
    {
        BCW = gameObject.AddComponent<BrainCloudWrapper>();
        BCW.Init();
        BCW.AuthenticateAnonymous((string jsonresponse, object cbobject) =>
        {
            Debug.Log("ok");
        }, (int status, int reasoncode, string jsonerror, object cbobject) =>
        {
            Debug.Log("failed");
        });
    }

    public void OnSubmit()
    {
        // OutputField.text += "<color=\"yellow\">" + InputField.text + "</color>\n\n";
        OutputField.text += InputField.text + "\n\n";

        var inputPrompt = InputField.text;
        InputField.text = null;

        converstaionHistory.Add(inputPrompt);
        RequestCompletion(ContactConversationHistory(), response =>
        {
            response = response.Trim("\r\n ".ToCharArray());
            // OutputField.text += "<color=\"lightblue\">" + response + "</color>\n\n";
            OutputField.text += response + "\n\n";
            converstaionHistory.Add(response);
            ScollRect.verticalNormalizedPosition = 0;
        }, error =>
        {
            // OutputField.text += "<color=\"red\">" + error + "</color>\n\n";
            OutputField.text += error + "\n\n";
            ScollRect.verticalNormalizedPosition = 0;
        });

        InputField.Select();
        InputField.ActivateInputField();
    }

    string ContactConversationHistory()
    {
        int cnt = 0;
        var i = converstaionHistory.Count - 1;
        for (; i >= 0 && cnt < MAX_PAYLOAD; i--)
            cnt += converstaionHistory[i].Length;

        if (i >= 0)
            converstaionHistory.RemoveRange(0, i + 1);

        var prompt = new StringBuilder();
        foreach (var l in converstaionHistory)
            prompt.AppendLine(l);

        return prompt.Replace('\r', ' ').Replace('\n', ' ').ToString();
    }

    void RequestCompletion(string prompt, Action<string> onResponse, Action<string> onError)
    {
        try
        {
            // StartCoroutine(RequestFromChatGPT(prompt, onResponse, onError));
            RequestFromBC(prompt, onResponse, onError);
        }
        catch(Exception ex)
        {
            onError?.Invoke("[Exception: " + ex.Message + "]");
        }
    }

    void RequestFromBC(string prompt, Action<string> onResponse, Action<string> onError)
    {
        BCW.ScriptService.RunScript("chatgpt_completions", "{\"prompt\": \"" + prompt + "\", \"max_tokens\":" + MAX_PAYLOAD + "}", (string jsonResponse, object cbObject) =>
        {
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            if (response["status"].ToString() == "200")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(response["data"].ToString());
                onResponse?.Invoke(data["response"]);
            }
            else if (converstaionHistory.Count > 0)
            {
                converstaionHistory.RemoveRange(0, converstaionHistory.Count >= 2 ? 2 : converstaionHistory.Count);
                RequestFromBC(ContactConversationHistory(), onResponse, onError);
            }
            else
                onError?.Invoke("[ChatBot Error: " + response["status"] + "]");
        }, (int status, int reasonCode, string jsonError, object cbObject) =>
        {
            onError?.Invoke("[BC Error: " + status + ", " + reasonCode + "]");
        });
    }

    IEnumerator RequestFromChatGPT(string prompt, Action<string> onResponse, Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(END_POINT, ""))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {API_KEY}");

            string json = "{\"prompt\":\"" + prompt + "\", \"max_tokens\":" + MAX_PAYLOAD + ", \"temperature\":0, \"model\":\"" + MODEL + "\"}";
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            {
                var responseJson = www.downloadHandler.text;
                var responseObj = JsonConvert.DeserializeObject<Response>(responseJson);
                var response = responseObj.choices[0]["text"];
                onResponse?.Invoke(response.Trim(' ', '\r', '\n'));
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError && www.responseCode == BAD_REQUEST && converstaionHistory.Count > 0)
            {
                // try again with a shorter prompt
                converstaionHistory.RemoveRange(0, converstaionHistory.Count >= 2 ? 2 : converstaionHistory.Count);
                yield return new WaitForSeconds(1);
                yield return RequestFromChatGPT(ContactConversationHistory(), onResponse, onError);
            }
            else
                onError?.Invoke("[ChatBot Error: " + www.error + "]");
        }
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && InputField.text != null && InputField.text.Trim() != "")
            OnSubmit();
    }

    class Response
    {
        public Dictionary<string, string>[] choices;
    }
}