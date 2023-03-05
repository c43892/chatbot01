using System;
using System.Collections;
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
    public Speaking Speaking;

    public Color ColorMe = Color.white;
    public Color ColorAI = Color.yellow;
    public Color ColorError = Color.red;

    private readonly List<KeyValuePair<Peer, string>> history = new();
    private readonly Dictionary<KeyValuePair<Peer, string>, AudioInfo> audioInfos = new();

    private string formatColor(Color c)
    {
        return string.Format("#{0:X02}{1:X02}{2:X02}{3:X02}", (int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255), (int)(c.a * 255));
    }

    public override void AddSentence(Peer p, string content)
    {
        history.Add(new(p, content));
        var color = p == Peer.user ? ColorMe : (p == Peer.assistant ? ColorAI : ColorError);
        var speaking = Instantiate(Speaking);
        speaking.Uncopied.gameObject.SetActive(p == Peer.assistant);
        speaking.Copied.gameObject.SetActive(false);
        speaking.IconMe.gameObject.SetActive(p == Peer.user);
        speaking.IconError.gameObject.SetActive(p == Peer.error);
        speaking.transform.SetParent(Speaking.transform.parent);
        speaking.gameObject.SetActive(true);
        speaking.Text.color = color;
        speaking.Text.text = content;

        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        ScollRect.verticalNormalizedPosition = 0;
    }

    public override IEnumerable<KeyValuePair<Peer, string>> History(Func<KeyValuePair<Peer, string>, bool> filter)
    {
        return history.Where(filter);
    }

    public override void OnError(string error)
    {
        AddSentence(Peer.error, error);
    }
}
