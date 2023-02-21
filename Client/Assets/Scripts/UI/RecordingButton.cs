using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RecordingButton : PressButton
{
    [System.Serializable]
    public class RecordingDoneEvent : UnityEvent<float> { }

    public Image RecordingRing;
    public RecordingDoneEvent OnRecordingDone;

    private float maxRecordingSecs = 10;

    private float recordingSecsLeft = 0;

    private void Awake()
    {
        RecordingRing.fillAmount = 0;
    }

    public void StartRecording(float maxTime)
    {
        if (Mathf.Approximately(maxTime, Mathf.Epsilon))
            return;

        maxRecordingSecs = maxTime;
        recordingSecsLeft = maxRecordingSecs;
        RecordingRing.fillAmount = 0;
        OnButtonUp.AddListener(StopRecording);
    }

    public void StopRecording()
    {
        var time = maxRecordingSecs - recordingSecsLeft;
        time = time > maxRecordingSecs ? maxRecordingSecs : time;
        OnRecordingDone?.Invoke(time);

        OnButtonUp.RemoveListener(StopRecording);
        recordingSecsLeft = 0;
        RecordingRing.fillAmount = 0;
    }

    private void Update()
    {
        if (recordingSecsLeft > 0)
        {
            recordingSecsLeft -= Time.deltaTime;
            if (recordingSecsLeft <= 0)
                StopRecording();
            else
                RecordingRing.fillAmount = (maxRecordingSecs - recordingSecsLeft) / maxRecordingSecs;
        }
    }
}
