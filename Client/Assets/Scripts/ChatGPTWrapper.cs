using Assets.Scripts;
using Assets.Scripts.Languages;
using Assets.Scripts.Services.BrainCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IConversationDialog;
using IServiceProvider = Assets.Scripts.Services.IServiceProvider;

public class ChatGPTWrapper : MonoBehaviour
{
    public AudioSource MainAudioSource;

    public RecordingButton RecordingBtn = null;
    public int AudioDeviceIndex = 0;
    public int MaxRecordingTime = 10;

    public ConversationDialog ConversationDialog;

    private IServiceProvider sp = null;

    public void Start()
    {
        //langMgr = new LanguageManager("cn", new()
        //{
        //    { "cn", new() { Code = "cmn_CN", Model = "cmn-CN-Standard-D" } },
        //    { "en", new() { Code = "en-US", Model = "en-US-Neural2-G" } },
        //    { "fr", new() { Code = "fr-FR", Model = "fr-FR-Neural2-A" } }
        //});

        var bcsp = new BrainCloudServiceProvider();
        bcsp.Init(gameObject.AddComponent<BrainCloudWrapper>(), () =>
        {
            sp = bcsp;
            Debug.Log("BrainCLoud init ok");
        }, (status, errorCode) =>
        {
            Debug.LogError("BrainCloud init failed: " + status + ":" + errorCode);
        });
    }

    AudioClip recordingClip;
    public void StartRecording()
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        recordingClip = Microphone.Start(microphoneName, true, MaxRecordingTime, 16000);
        RecordingBtn.StartRecording();
    }

    public void OnStopRecording(float timeRecorded, byte[] audioData)
    {
        void onError(string error)
        {
            ConversationDialog.OnError(error);
        }

        var srcLang = LanguageCode.cn; // Chinese input
        sp.GetSpeech2TextService(srcLang).Speech2Text(audioData, recordingClip.frequency, recordingClip.channels, (question, langCode, confidence) =>
        {
            question = question.Trim("\r\n ".ToCharArray());
            ConversationDialog.AddSentence(Peer.user, question);

            sp.GetChatBotService(2048).Chat(ConversationDialog.History((kv) => kv.Key  == Peer.assistant || kv.Key == Peer.user).Select((kv) =>
            {
                return kv.Key switch
                {
                    Peer.user => new KeyValuePair<string, string>("user", kv.Value),
                    Peer.assistant => new KeyValuePair<string, string>("assistant", kv.Value),
                    _ => new KeyValuePair<string, string>(null, null), // should never happen
                };
            }), answer =>
            {
                answer = answer.Trim("\r\n ".ToCharArray());
                ConversationDialog.AddSentence(Peer.assistant, answer);

                var dstLang = LanguageDetector.DetectLanguage(answer);
                sp.GetText2SpeechService(dstLang, 16000).Text2Speech(answer, 16000, audioData =>
                {
                    // construct the audio clip
                    var clipData = AudioTool.WavData2ClipData(audioData);
                    var clip = AudioClip.Create("AnswerClip", audioData.Length / 2, 1, 16000, false);
                    clip.SetData(clipData, 0);

                    // play the audio
                    MainAudioSource.clip = clip;
                    MainAudioSource.Play();

                    StartCoroutine(Wait4Condition(
                        () => MainAudioSource.isPlaying && MainAudioSource.time < clip.length,
                        ConversationDialog.OnAudioEnded
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
}
