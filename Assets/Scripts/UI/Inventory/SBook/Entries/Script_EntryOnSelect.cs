using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Script_Entry))]
public class Script_EntryOnSelect : MonoBehaviour, ISelectHandler
{
    public Script_EntriesViewController controller;
    
    /// <summary>
    /// called automatically when entry is selected
    /// </summary>
    public void OnSelect(BaseEventData e)
    {
        // call controller with appropriate data
        string text = GetComponent<Script_Entry>().text;
        controller.OnEntrySelect(text);
    }

    public void Setup(Script_EntriesViewController _controller)
    {
        controller = _controller;
    }
}
