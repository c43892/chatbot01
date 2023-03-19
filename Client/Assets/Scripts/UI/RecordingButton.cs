using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RecordingButton : PressButton
{
    [System.Serializable]
    public class RecordingDoneEvent : UnityEvent<float, byte[]> { }

    public UnityEvent OnRecordingStarted;
    public RecordingDoneEvent OnRecordingDone;

    public Image RecordingRing;

    public int SampleRate = 16000;
    public int MaxRecordingSecs = 10;
    public int AudioDeviceIndex = 0;

    private AudioClip recordingClip;
    private float recordingSecsLeft = 0;

    private void Awake()
    {
        RecordingRing.fillAmount = 0;
        OnButtonDown.AddListener(StartRecording);
    }

    public void StartRecording()
    {
        recordingSecsLeft = MaxRecordingSecs;
        RecordingRing.fillAmount = 0;

        string microphoneName = Microphone.devices[AudioDeviceIndex];
        recordingClip = Microphone.Start(microphoneName, true, MaxRecordingSecs, SampleRate);

        OnButtonUp.AddListener(StopRecording);
        OnRecordingStarted?.Invoke();
    }

    public void StopRecording()
    {
        if (recordingSecsLeft <= 0)
            return;

        var time = MaxRecordingSecs - recordingSecsLeft;
        time = time > MaxRecordingSecs ? MaxRecordingSecs : time;

        OnButtonUp.RemoveListener(StopRecording);
        recordingSecsLeft = 0;
        RecordingRing.fillAmount = 0;

        string microphoneName = Microphone.devices[AudioDeviceIndex];
        Microphone.End(microphoneName);

        var samples = (int)Math.Ceiling(time * recordingClip.samples / MaxRecordingSecs);
        float[] clipData = new float[samples];
        recordingClip.GetData(clipData, 0);
        var audioData = AudioTool.ClipData2WavData(clipData);

        OnRecordingDone?.Invoke(time, audioData);
    }

    private void Update()
    {
        if (recordingSecsLeft > 0)
        {
            recordingSecsLeft -= Time.deltaTime;
            if (recordingSecsLeft <= 0)
                StopRecording();
            else
                RecordingRing.fillAmount = (MaxRecordingSecs - recordingSecsLeft) / MaxRecordingSecs;
        }
    }
}
