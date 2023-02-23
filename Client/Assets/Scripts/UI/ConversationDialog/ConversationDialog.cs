using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IConversationDialog;

public interface IConversationDialog
{
    public class AudioInfo
    {
        public byte[] audioData;
        public int sampleRate;
        public int channels;
    }

    public enum Peer
    {
        Me,
        AI,
        Error,
    }

    void AddSentence(Peer p, string content);

    IEnumerable<KeyValuePair<Peer, string>> ReversedHistory(Func<KeyValuePair<Peer, string>, bool> filter);

    void AddAudio(Peer p, string content, AudioInfo audioInfo);

    AudioInfo GetAudio(Peer p, string content);

    void OnError(string error);

    void OnAudioEnded();
}

public abstract class ConversationDialog : MonoBehaviour, IConversationDialog
{
    public abstract void AddSentence(Peer id, string content);

    public abstract IEnumerable<KeyValuePair<Peer, string>> ReversedHistory(Func<KeyValuePair<Peer, string>, bool> filter);

    public abstract void AddAudio(Peer id, string content, AudioInfo audioInfo);

    public abstract AudioInfo GetAudio(Peer id, string content);

    public virtual void OnError(string error) { }

    public virtual void OnAudioEnded() { }
}