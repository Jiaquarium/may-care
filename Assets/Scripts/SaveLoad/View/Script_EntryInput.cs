using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Script_EntryInput : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject submitButton;
    public Script_EntryManager entryManager;
    
    private TMP_InputField TMPInputField;
    private TextMeshProUGUI TMPGUI;
    private float caretBlinkRate;
    private Color caretColor;

    void Update()
    {
        HandleKeyInput();
    }

    void HandleKeyInput()
    {
        if (
            Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetButtonDown(Const_KeyCodes.Submit)
            || Input.GetButtonDown(Const_KeyCodes.Cancel)
        )
        {
            TMPGUI.GetComponent<TMP_InputField>().DeactivateInputField();
            // set Submit as selected
            EventSystem.current.SetSelectedGameObject(submitButton);
        }
    }
    
    public void OnSelect(BaseEventData e)
    {
        // set cursor to end when we initialize with existing entry (this already happens on deselect)
        TMPInputField.caretPosition = TMPInputField.text.Length;
        SetCaretVisible(true);
    }

    public void OnDeselect(BaseEventData e)
    {
        SetCaretVisible(false);
    }

    void SetCaretVisible(bool isOn)
    {
        if (isOn)
        {
            TMPInputField.caretBlinkRate   = caretBlinkRate;
            TMPInputField.caretColor       = caretColor;
        }
        else
        {
            TMPInputField.caretBlinkRate   = 0f;
            Color newCaretColor = caretColor;
            newCaretColor.a = 0f;
            TMPInputField.caretColor       = newCaretColor;
        }
    }

    public void InitializeState(string nameId)
    {
        // check to see if editting or new entry
        Script_Entry existingEntry = entryManager.GetExistingEntry(nameId);
        if (existingEntry != null)
        {
            TMPInputField.text = existingEntry.text;
            return;
        }
        
        TMPInputField.text = "";
    }

    public void Setup()
    {
        TMPGUI = GetComponent<TextMeshProUGUI>();
        TMPInputField = GetComponent<TMP_InputField>();
        caretBlinkRate = TMPInputField.caretBlinkRate;
        caretColor = TMPInputField.caretColor;
    }
}
