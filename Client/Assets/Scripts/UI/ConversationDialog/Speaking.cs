using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speaking : MonoBehaviour
{
    public Button Uncopied;
    public Button Copied;
    public Image IconMe;
    public Image IconError;
    public Text Text;

    TextEditor textEditor = null;
    public void OnCopyText()
    {
        Uncopied.gameObject.SetActive(false);
        Copied.gameObject.SetActive(true);

        if (textEditor == null)
            textEditor = new();

        textEditor.text = Text.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }
}
