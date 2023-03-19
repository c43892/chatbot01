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
        system,
        user,
        assistant,
        error,
    }

    void AddSentence(Peer p, string content);

    IEnumerable<KeyValuePair<Peer, string>> History(Func<KeyValuePair<Peer, string>, bool> filter);

    void OnError(string error);
}

public abstract class ConversationDialog : MonoBehaviour, IConversationDialog
{
    public abstract void AddSentence(Peer id, string content);

    public abstract IEnumerable<KeyValuePair<Peer, string>> History(Func<KeyValuePair<Peer, string>, bool> filter);

    public virtual void OnError(string error) { }

    public virtual void OnAudioEnded() { }
}