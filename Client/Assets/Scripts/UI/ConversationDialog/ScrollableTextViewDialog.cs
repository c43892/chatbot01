using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static IConversationDialog;
using Color = UnityEngine.Color;

class ScrollableTextViewDialog : ConversationDialog
{
    public ScrollRect ScollRect;
    public Text TextView;

    public Color ColorMe = Color.white;
    public Color ColorAI = Color.yellow;
    public Color ColorError = Color.red;

    private readonly List<KeyValuePair<Peer, string>> history = new();
    private readonly Dictionary<KeyValuePair<Peer, string>, AudioInfo> audioInfos = new();

    private string formatColor(Color c)
    {
        return string.Format("{0:00}{1:00}{2:00}{3:00}", (int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255), (int)(c.a * 255));
    }

    public override void AddSentence(Peer p, string content)
    {
        history.Add(new(p, content));
        var color = p == Peer.Me ? ColorMe : (p == Peer.AI ? ColorAI : ColorError);
        TextView.text +=  "<color=" + formatColor(color) + ">"+ content + "</color>\n\n";
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

    public override void OnError(string error)
    {
        AddSentence(Peer.Error, error);
    }
}
