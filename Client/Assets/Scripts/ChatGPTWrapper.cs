using Assets.Scripts;
using Assets.Scripts.Languages;
using Assets.Scripts.Services;
using Assets.Scripts.Services.BrainCloud;
using BrainCloud;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using IServiceProvider = Assets.Scripts.Services.IServiceProvider;

public class ChatGPTWrapper : MonoBehaviour
{
    readonly List<string> converstaionHistory = new();

    public Text ConversationOutput;
    public ScrollRect ScollRect;
    public AudioSource MainAudioSource;
    public LangaugeSelection LangSel;

    private IServiceProvider sp = null;
    private LanguageManager langMgr = null;

    public int AudioDeviceIndex = 0;

    public void Start()
    {
        langMgr = new LanguageManager("fr", new()
        {
            { "en", new() { Code = "en-US", Model = "en-US-Neural2-G" } },
            { "cn", new() { Code = "cmn_CN", Model = "cmn-CN-Standard-D" } },
            { "fr", new() { Code = "fr-FR", Model = "fr-FR-Neural2-A" } }
        });

        var bcsp = new BrainCloudServiceProvider();
        bcsp.Init(gameObject.AddComponent<BrainCloudWrapper>(), langMgr.DefaultLanguageInfo, () =>
        {
            sp = bcsp;
            Debug.Log("BrainCLoud init ok");
        }, (status, errorCode) =>
        {
            Debug.LogError("BrainCloud init failed: " + status + ":" + errorCode);
        });

        LangSel.SetLanguage(langMgr.DefaultLangauge);
    }

    AudioClip recordingClip;
    public void StartRecording()
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        recordingClip = Microphone.Start(microphoneName, true, 5, 16000);
    }

    public void StopRecording()
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        Microphone.End(microphoneName);

        float[] clipData = new float[recordingClip.samples];
        recordingClip.GetData(clipData, 0);
        var audioData = AudioTool.ClipData2WavData(clipData);

        void onError(string error)
        {
            ConversationOutput.text += error + "\n";
            ScollRect.verticalNormalizedPosition = 0;
        }

        sp.GetSpeech2TextService().Speech2Text(audioData, recordingClip.frequency, recordingClip.channels, (question, langCode, confidence) =>
        {
            ConversationOutput.text += question + "\n";
            converstaionHistory.Add(question);
            ScollRect.verticalNormalizedPosition = 0;

            sp.GetChatBotService(2048).Ask(question, answer =>
            {
                ConversationOutput.text += "<color=yellow>" + answer + "</color>\n\n";
                converstaionHistory.Add(answer);
                ScollRect.verticalNormalizedPosition = 0;

                sp.GetText2SpeechService(16000).Text2Speech(answer, audioData =>
                {
                    clipData = AudioTool.WavData2ClipData(audioData);
                    var clip = AudioClip.Create("AnswerClip", audioData.Length / 2, 1, 16000, false);
                    clip.SetData(clipData, 0);
                    MainAudioSource.clip = clip;
                    MainAudioSource.Play();
                }, onError);
            }, onError);
        }, onError);
    }

    public void OnLanguageChanged(string lang)
    {
        sp.LangInfo = langMgr[lang];
    }
}
