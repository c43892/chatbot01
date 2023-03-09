using Assets.Scripts.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LangaugeSelectionPanel : MonoBehaviour
{
    public Transform Root;
    public Transform IconContainer;
    public Button IconModel;

    [Serializable]
    public class LanguageIconPair
    {
        public LanguageCode Language;
        public Sprite Icon;
    }

    public LanguageIconPair[] AllLanguageIcons;

    Dictionary<LanguageCode, Sprite> AllIcons = null;

    private void Awake()
    {
        PrepareIcons();
    }

    public void OpenSelection(IEnumerable<LanguageCode> allSupported, LanguageCode defaultLang, Action<LanguageCode, Sprite> onSelected)
    {
        Root.gameObject.SetActive(true);

        while (IconContainer.childCount > 0)
        {
            var child = IconContainer.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        foreach (var lang in allSupported)
        {
            var icon = Instantiate(IconModel);
            icon.gameObject.SetActive(true);
            icon.transform.SetParent(IconContainer);

            var sripte = AllIcons[lang];
            icon.GetComponent<Image>().sprite = sripte;

            icon.onClick.AddListener(() =>
            {
                Root.gameObject.SetActive(false);
                onSelected?.Invoke(lang, sripte);
            });
        }
    }

    public Sprite GetSprite(LanguageCode lang)
    {
        return AllIcons[lang];
    }

    void PrepareIcons()
    {
        if (AllIcons != null)
            return;

        AllIcons = new Dictionary<LanguageCode, Sprite>();
        foreach (var pair in AllLanguageIcons)
            AllIcons.Add(pair.Language, pair.Icon);
    }

    public void CancelSelection()
    {
        Root.gameObject.SetActive(false);
    }
}