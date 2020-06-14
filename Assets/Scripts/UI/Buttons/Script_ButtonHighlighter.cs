using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Script_ButtonHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image[] outlines;
    [SerializeField]
    private bool isHighlighted;
    
    void Awake()
    {
        InitializeState();
    }

    void OnDisable() {
        InitializeState();
    }

    public void OnSelect(BaseEventData e)
    {
        HighlightOutline(true);
    }

    public void OnDeselect(BaseEventData e)
    {
        HighlightOutline(false);
    }

    void InitializeState()
    {
        isHighlighted = true;
        HighlightOutline(false);
    }

    void HighlightOutline(bool isOn)
    {
        if (isHighlighted == isOn)  return;
        
        foreach (Image img in outlines)
        {
            img.enabled = isOn;
        }
        
        isHighlighted = isOn;
    }
}
