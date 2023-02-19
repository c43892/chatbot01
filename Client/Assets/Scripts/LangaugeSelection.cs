using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LangaugeSelection : MonoBehaviour
{
    public string[] Languages;
    public TextMeshProUGUI LanguageCode;

    public UnityEvent<string> OnLanguageChanged;

    int curSel = 0;

    public void SetLanguage(string lang)
    {
        foreach (var language in Languages)
        {
            if (language == lang)
            {
                LanguageCode.text = lang;
                return;
            }
        }
    }

    public void OnButtonClicked()
    {
        curSel = (curSel + 1) % Languages.Length;
        var langCode = Languages[curSel];
        LanguageCode.text = langCode;
        OnLanguageChanged?.Invoke(langCode);
    }
}
