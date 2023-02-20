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
using System.Linq;
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
    readonly List<KeyValuePair<string, string>> converstaionHistory = new();

    public Text ConversationOutput;
    public ScrollRect ScollRect;
    public AudioSource MainAudioSource;
    public LangaugeSelection LangSel;

    private IServiceProvider sp = null;
    private LanguageManager langMgr = null;
    private string curLang = null;

    public int AudioDeviceIndex = 0;

    public void Start()
    {
        langMgr = new LanguageManager("cn", new()
        {
            { "cn", new() { Code = "cmn_CN", Model = "cmn-CN-Standard-D" } },
            { "en", new() { Code = "en-US", Model = "en-US-Neural2-G" } },
            { "fr", new() { Code = "fr-FR", Model = "fr-FR-Neural2-A" } }
        });

        var bcsp = new BrainCloudServiceProvider();
        bcsp.Init(gameObject.AddComponent<BrainCloudWrapper>(), () =>
        {
            sp = bcsp;
            Debug.Log("BrainCLoud init ok");
        }, (status, errorCode) =>
        {
            Debug.LogError("BrainCloud init failed: " + status + ":" + errorCode);
        });

        LangSel.SetLanguage(langMgr.DefaultLangauge);
        curLang = langMgr.DefaultLangauge;
    }

    AudioClip recordingClip;
    public void StartRecording()
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        recordingClip = Microphone.Start(microphoneName, true, 10, 16000);
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

        var langQestion = langMgr[curLang];
        sp.GetSpeech2TextService(langQestion.Code).Speech2Text(audioData, recordingClip.frequency, recordingClip.channels, (question, langCode, confidence) =>
        {
            ConversationOutput.text += question + "\n";
            converstaionHistory.Add(new("question", question));
            ScollRect.verticalNormalizedPosition = 0;

            // assembe the conversition history
            var prompt = "";
            for (var i = converstaionHistory.Count - 1; i >= 0; i--)
            {
                var newLine = converstaionHistory[i].Value;
                if (prompt.Length + newLine.Length < 2048)
                    prompt = newLine + "\n" + prompt;
                else
                    break;
            }

            sp.GetChatBotService(2048).Ask(prompt, answer =>
            {
                ConversationOutput.text += "<color=yellow>" + answer + "</color>\n\n";
                converstaionHistory.Add(new("answer:", answer));
                ScollRect.verticalNormalizedPosition = 0;

                var langCodeAnswer = (new LanguageDetector()).DetectLanguage(answer);
                var langAnswer = langMgr[langCodeAnswer];
                sp.GetText2SpeechService(langAnswer.Code, langAnswer.Model, 16000).Text2Speech(answer, audioData =>
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
        curLang = lang;
    }
}
