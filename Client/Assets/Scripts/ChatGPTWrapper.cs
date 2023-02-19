using Assets.Scripts;
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

public class ChatGPTWrapper : MonoBehaviour
{
    const string END_POINT = "https://api.openai.com/v1/completions";
    const string OPENAI_API_KEY = "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";
    const string MODEL = "text-davinci-003";
    const int MAX_PAYLOAD = 2048;
    const int BAD_REQUEST = 400;

    private string GOOGLE_API_KEY = "AIzaSyD8sHeh_dcU8OYFo3NcUKVaFQk1ylIko8Q";
    private string GOOGLE_TEXT_TO_SPEECH_API = "https://texttospeech.googleapis.com/v1/text:synthesize";
    private string GOOGLE_SPEECH_TO_TEXT_API = "https://speech.googleapis.com/v1/speech:recognize";


    readonly List<string> converstaionHistory = new();

    public InputField OutputField;
    public InputField InputField;
    public ScrollRect ScollRect;
    public AudioSource MainAudioSource;

    const string BRAIN_CLOUD_SERVICE_URL = "https://portal.braincloudservers.com";
    const string BRAIN_CLOUD_APP_ID = "14407";
    const string BRAIN_CLOUD_APP_SECRET = "84b17cdb-c2b1-4304-b401-6b1d7d4e4165";

    private BrainCloudServiceProvider BCP = new BrainCloudServiceProvider();
    private IText2SpeechService text2SpeechService;
    private ISpeech2TextService speech2TextService;
    private IChatBotService chatBotService;

    public int AudioDeviceIndex = 0;

    public void Start()
    {
        // BCP.Init(gameObject.AddComponent<BrainCloudWrapper>(), "en-us", "en-US-Neural2-G", () =>
        BCP.Init(gameObject.AddComponent<BrainCloudWrapper>(), "cmn-CN", "cmn-CN-Wavenet-A", () =>
        {
            text2SpeechService = BCP.GetText2SpeechService(16000);
            speech2TextService = BCP.GetSpeech2TextService();
            chatBotService = BCP.GetChatBotService(MAX_PAYLOAD);
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
        recordingClip = Microphone.Start(microphoneName, true, 5, 16000);
    }

    public void StopRecording()
    {
        string microphoneName = Microphone.devices[AudioDeviceIndex];
        Microphone.End(microphoneName);

        float[] clipData = new float[recordingClip.samples];
        recordingClip.GetData(clipData, 0);
        var audioData = AudioTool.ClipData2AudioData(clipData);

        void onError(string error)
        {
            OutputField.text += error + "\n";
            ScollRect.verticalNormalizedPosition = 0;
        }

        speech2TextService.Speech2Text(audioData, recordingClip.frequency, recordingClip.channels, (question, langCode, confidence) =>
        {
            OutputField.text += question + "\n";
            converstaionHistory.Add(question);
            ScollRect.verticalNormalizedPosition = 0;

            chatBotService.Ask(question, answer =>
            {
                OutputField.text += answer + "\n";
                converstaionHistory.Add(answer);
                ScollRect.verticalNormalizedPosition = 0;

                text2SpeechService.Text2Speech(answer, audioData =>
                {
                    clipData = AudioTool.AudioData2ClipData(audioData);
                    var clip = AudioClip.Create("AnswerClip", audioData.Length / 2, 1, text2SpeechService.SampleRate, false);
                    clip.SetData(clipData, 0);
                    MainAudioSource.clip = clip;
                    MainAudioSource.Play();
                }, onError);
            }, onError);
        }, onError);
    }
}
