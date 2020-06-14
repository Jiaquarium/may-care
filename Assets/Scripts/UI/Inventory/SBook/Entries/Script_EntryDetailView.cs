using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Script_EntryDetailView : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetText(string s)
    {
        text.text = s;
    }
}
