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

public class AppTranslator : MonoBehaviour
{
    public AudioSource AudioSrouce;
    public RecordingButton RecordingButtonA;
    public RecordingButton RecordingButtonB;
    public Image LangaugeIconA;
    public Image LangaugeIconB;
    public Text TranscriptA;
    public Text TranscriptB;
    public LangaugeSelectionPanel langSelPanel;
    public IAPManager IapMgr;

    private IServiceProvider sp = null;
    private LanguageManager langMgr = null;
    private LanguageCode langA = LanguageCode.cn;
    private LanguageCode langB = LanguageCode.en;

    private void Awake()
    {
        RecordingButtonA.OnRecordingStarted.AddListener(OnRecAStarted);
        RecordingButtonB.OnRecordingStarted.AddListener(OnRecBStarted);

        RecordingButtonA.OnRecordingDone.AddListener(OnRecADone);
        RecordingButtonB.OnRecordingDone.AddListener(OnRecBDone);
    }

    public void Start()
    {
        langMgr = new LanguageManager(new LanguageCode[]
        {
            LanguageCode.cn,
            LanguageCode.en,
            LanguageCode.fr,
            LanguageCode.ja
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

        TranscriptA.text = TranscriptB.text = "";
    }

    void onError(string error)
    {
        Debug.LogError(error);
    }

    public void OnRecAStarted()
    {
        TranscriptA.text = "";
    }

    public void OnRecADone(float time, byte[] audioData)
    {
        Speech2SpeechTranslation(langA, langB, audioData, RecordingButtonA.SampleRate,
            srcText => TranscriptA.text = srcText,
            dstText => TranscriptB.text = dstText,
            (audioData) => {
                var audioClip = AudioData2AudioClip(audioData, RecordingButtonA.SampleRate);
                AudioSrouce.clip = audioClip;
                AudioSrouce.Play();

                StartCoroutine(Wait4Condition(
                    () => AudioSrouce.isPlaying && AudioSrouce.time < audioClip.length,
                    null));
            }, onError);
    }

    public void OnRecBStarted()
    {
        TranscriptB.text = "";
    }

    public void OnRecBDone(float time, byte[] audioData)
    {
        Speech2SpeechTranslation(langB, langA, audioData, RecordingButtonB.SampleRate,
            srcText => TranscriptB.text = srcText,
            dstText => TranscriptA.text = dstText,
            (audioData) => {
            var audioClip = AudioData2AudioClip(audioData, RecordingButtonB.SampleRate);
                AudioSrouce.clip = audioClip;
                AudioSrouce.Play();

                StartCoroutine(Wait4Condition(
                    () => AudioSrouce.isPlaying && AudioSrouce.time < audioClip.length,
                    null));
            }, onError);
    }

    IEnumerator Wait4Condition(Func<bool> condition, Action callback)
    {
        while (condition())
            yield return new WaitForEndOfFrame();

        callback?.Invoke();
    }

    public void Speech2SpeechTranslation(LanguageCode srcLang, LanguageCode dstLang, byte[] srcAudioData, int sampleRate, Action<string> onSrcText, Action<string> onDstText, Action<byte[]> onResponse, Action<string> onError)
    {
        sp.GetSpeech2TextService(srcLang).Speech2Text(srcAudioData, sampleRate, 1, (srcText, _, confidence) =>
        {
            srcText = srcText.Trim("\r\n ".ToCharArray());
            onSrcText?.Invoke(srcText);
            sp.GetTranslationService().Translate(srcText, srcLang, dstLang, (dstText) =>
            {
                onDstText?.Invoke(dstText);
                sp.GetText2SpeechService(dstLang, sampleRate).Text2Speech(dstText, sampleRate, onResponse, onError);
            }, onError);
        }, onError);
    }

    AudioClip AudioData2AudioClip(byte[] audioData, int sampleRate)
    {
        var dstClipData = AudioTool.WavData2ClipData(audioData);
        var clip = AudioClip.Create("AnswerClip", dstClipData.Length, 1, sampleRate, false);
        clip.SetData(dstClipData, 0);

        return clip;
    }

    public void OnChangeLanaguageA()
    {
        langSelPanel.OpenSelection(langMgr.Languages, langMgr.Languages[0], (lang, sprite) =>
        {
            langA = lang;
            LangaugeIconA.sprite = sprite;
        });
    }

    public void OnChangeLanaguageB()
    {
        langSelPanel.OpenSelection(langMgr.Languages, langMgr.Languages[1], (lang, sprite) =>
        {
            langB = lang;
            LangaugeIconB.sprite = sprite;
        });
    }

    public void OnIapButton()
    {
        IapMgr.ShowPurchaseView();
    }
}
