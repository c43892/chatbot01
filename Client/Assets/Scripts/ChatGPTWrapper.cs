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
using static IConversationDialog;
using IServiceProvider = Assets.Scripts.Services.IServiceProvider;

public class ChatGPTWrapper : MonoBehaviour
{
    public AudioSource MainAudioSource;
    public LangaugeSelection LangSel;

    public int AudioDeviceIndex = 0;
    public int MaxRecordingTime = 10;

    public ConversationDialog ConversationDialog;

    private IServiceProvider sp = null;
    private LanguageManager langMgr = null;
    private string curLang = null;

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
        recordingClip = Microphone.Start(microphoneName, true, MaxRecordingTime, 16000);
    }

    public void StopRecording(float timeRecorded)
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        Microphone.End(microphoneName);

        var samples = (int)Math.Ceiling(timeRecorded * recordingClip.samples / MaxRecordingTime);
        float[] clipData = new float[samples];
        recordingClip.GetData(clipData, 0);
        var audioData = AudioTool.ClipData2WavData(clipData);

        void onError(string error)
        {
            ConversationDialog.OnError(error);
        }

        var langQestion = langMgr[curLang];
        sp.GetSpeech2TextService(langQestion.Code).Speech2Text(audioData, recordingClip.frequency, recordingClip.channels, (question, langCode, confidence) =>
        {
            question = question.Trim("\r\n ".ToCharArray());
            ConversationDialog.AddSentence(Peer.user, question);

            // assembe the conversition history, that's how chatgpt can keep the conversation context
            var prompt = "";
            foreach (var s in ConversationDialog.History((s) => true).Reverse())
            {
                var newLine = s.Value;
                if (prompt.Length + newLine.Length >= 2048)
                    break;

                prompt = newLine + "\n" + prompt;
            }

            sp.GetChatBotService(2048).Completion(prompt, answer =>
            {
                answer = answer.Trim("\r\n ".ToCharArray());
                ConversationDialog.AddSentence(Peer.assistant, answer);

                var langCodeAnswer = (new LanguageDetector()).DetectLanguage(answer);
                var langAnswer = langMgr[langCodeAnswer];
                sp.GetText2SpeechService(langAnswer.Code, langAnswer.Model, 16000).Text2Speech(answer, audioData =>
                {
                    // ConversationDialog.AddAudio(Peer.AI, answer, new() { audioData = audioData, sampleRate = 16000, channels = 1 });

                    // construct the audio clip
                    clipData = AudioTool.WavData2ClipData(audioData);
                    var clip = AudioClip.Create("AnswerClip", audioData.Length / 2, 1, 16000, false);
                    clip.SetData(clipData, 0);

                    // play the audio
                    MainAudioSource.clip = clip;
                    MainAudioSource.Play();

                    StartCoroutine(Wait4Condition(
                        () => MainAudioSource.isPlaying && MainAudioSource.time < clip.length, 
                        null // ConversationDialog.OnAudioEnded
                    ));
                }, onError);
            }, onError);
        }, onError);
    }

    IEnumerator Wait4Condition(Func<bool> condition, Action callback)
    {
        while (condition())
            yield return new WaitForEndOfFrame();

        callback?.Invoke();
    }

    public void OnLanguageChanged(string lang)
    {
        curLang = lang;
    }


    public InputField Input;

    public void OnSubmit()
    {
        void onError(string error)
        {
            ConversationDialog.OnError(error);
        }

        var question = Input.text.Trim("\r\n ".ToCharArray());
        Input.text = "";
        ConversationDialog.AddSentence(Peer.user, question);

        var history = ConversationDialog.History((s) => s.Key != Peer.error);
        var messages = history.Select((s) => new KeyValuePair<string, string>(s.Key.ToString(), s.Value));
        sp.GetChatBotService(2048).Chat(messages, answer =>
            {
                answer = answer.Trim("\r\n ".ToCharArray());
                ConversationDialog.AddSentence(Peer.assistant, answer);
            }, onError);
    }
}
