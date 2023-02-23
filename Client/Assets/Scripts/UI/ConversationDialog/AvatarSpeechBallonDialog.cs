using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static IConversationDialog;

public class AvatarSpeechBallonDialog : ConversationDialog
{
    public Text AIDialog;
    public Image DialogBg;
    public Text QuestionText;

    public float DialogFadeInOutTime = 0.5f;

    private readonly List<KeyValuePair<Peer, string>> history = new();
    private readonly Dictionary<KeyValuePair<Peer, string>, AudioInfo> audioInfos = new();

    private void Start()
    {
        var bgColor = DialogBg.color;
        bgColor.a = 0;
        DialogBg.color = bgColor;

        var aiTextColor = AIDialog.color;
        aiTextColor.a = 0;
        AIDialog.color = aiTextColor;

        var qTextColor = QuestionText.color;
        qTextColor.a = 0;
        QuestionText.color = qTextColor;
    }

    public override void AddSentence(Peer p, string content)
    {
        switch (p)
        {
            case Peer.AI:
                AITalk(content);
                break;
            case Peer.Me:
                ShowQuestion(content);
                break;
        }

        history.Add(new(p, content));
    }

    public override IEnumerable<KeyValuePair<Peer, string>> ReversedHistory(Func<KeyValuePair<Peer, string>, bool> filter)
    {
        return history.Where(filter).Reverse();
    }

    public override void AddAudio(Peer p, string content, AudioInfo audioInfo)
    {
        audioInfos[new(p, content)] = audioInfo;
    }

    public override AudioInfo GetAudio(Peer p, string content)
    {
        return audioInfos.ContainsKey(new(p, content)) ? audioInfos[new(p, content)] : null;
    }

    public override void OnAudioEnded()
    {
        if (AIDialogFadeInOutHandler != null)
            StopCoroutine(AIDialogFadeInOutHandler);

        AIDialogFadeInOutHandler = FadeInOut(new Graphic[] { DialogBg, AIDialog }, DialogFadeInOutTime, 1, 0);
        StartCoroutine(AIDialogFadeInOutHandler);

        if (QuestionFadeInOutHandler != null)
            StopCoroutine(QuestionFadeInOutHandler);

        QuestionFadeInOutHandler = FadeInOut(new Graphic[] { QuestionText }, DialogFadeInOutTime, 1, 0);
        StartCoroutine(QuestionFadeInOutHandler);
    }

    void AITalk(string text)
    {
        if (AIDialogFadeInOutHandler != null)
            StopCoroutine(AIDialogFadeInOutHandler);

        AIDialog.text = text;
        AIDialogFadeInOutHandler = FadeInOut(new Graphic[] { DialogBg, AIDialog }, DialogFadeInOutTime, 0, 1);
        StartCoroutine(AIDialogFadeInOutHandler);
    }

    void ShowQuestion(string text)
    {
        if (QuestionFadeInOutHandler != null)
            StopCoroutine(QuestionFadeInOutHandler);

        QuestionText.text = text;
        QuestionFadeInOutHandler = FadeInOut(new Graphic[] { QuestionText }, DialogFadeInOutTime, 0, 1);
        StartCoroutine(QuestionFadeInOutHandler);
    }

    IEnumerator QuestionFadeInOutHandler = null;
    IEnumerator AIDialogFadeInOutHandler = null;
    IEnumerator FadeInOut(IEnumerable<Graphic> gs, float time, float startAlpha, float endAlpha)
    {
        var dAlpha = endAlpha - startAlpha;
        var timeElapsed = 0f;

        foreach (var g in gs)
            g.color = new Color(g.color.r, g.color.g, g.color.b, startAlpha);

        while (timeElapsed < time)
        {
            foreach (var g in gs)
            {
                var a = startAlpha + dAlpha * timeElapsed / time;
                g.color = new Color(g.color.r, g.color.g, g.color.b, a);
            }

            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }

        foreach (var g in gs)
            g.color = new Color(g.color.r, g.color.g, g.color.b, endAlpha);
    }
}
