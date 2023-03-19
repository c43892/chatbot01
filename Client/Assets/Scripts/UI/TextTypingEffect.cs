using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TextTypingEffect : MonoBehaviour
{
    public Text TargetText;

    public string PlaceHolderText;
    public float InternalSecPerCharacter = 1.0f;

    bool activated = false;
    float timeElapsed = 0;

    public void StartAni(bool reset = true)
    {
        activated = true;
        if (reset)
        {
            TargetText.text = null;
            timeElapsed = 0;
        }
    }

    public void Stop(bool reset = true)
    {
        if (reset)
        {
            TargetText.text = null;
            timeElapsed = 0;
        }

        activated = false;
    }

    private void Update()
    {
        if (!activated || InternalSecPerCharacter <= 0 || PlaceHolderText == null || PlaceHolderText.Length == 0)
            return;

        timeElapsed += Time.deltaTime;
        var cnt = (int)(timeElapsed / InternalSecPerCharacter);
        cnt %= (PlaceHolderText.Length + 1);
        var str = PlaceHolderText.Substring(0, cnt);
        TargetText.text = str;
    }
}
