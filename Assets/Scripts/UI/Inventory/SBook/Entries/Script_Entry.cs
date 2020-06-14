using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Script_Entry : MonoBehaviour
{
    public int Id;
    public string text;
    public string nameId;
    public string timestamp;
    public string headline;

    [SerializeField] private TextMeshProUGUI timestampText;
    [SerializeField] private TextMeshProUGUI headlineText;

    public void Edit(string _text, string _timestamp)
    {
        text = _text;
        timestamp = _timestamp;

        timestampText.text = _timestamp;
    }
    
    public void Setup(
        int _Id,
        string _nameId,
        string _text,
        string _timestamp,
        string _headline
    )
    {
        Id = _Id;
        nameId = _nameId;
        text = _text;
        timestamp = _timestamp;
        headline = _headline;

        timestampText.text = _timestamp;
        headlineText.text = _headline;
    }
}
