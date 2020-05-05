using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_InputManager : MonoBehaviour
{
    public Text inputNameDisplay;
    private string inputName = "";
    private float m_TimeStamp;
    private bool cursor = false;
    private string cursorChar = "";
    private int maxStringLength = 16;

    void Update()
    {
        Cursor();
        TrimName();
        HandleKeyInput();
        DisplayInputName();
    }

    void DisplayInputName()
    {
        inputNameDisplay.text = inputName + cursorChar;
    }

    void Cursor()
    {
        if (Time.time - m_TimeStamp >= 0.5)
        {
            m_TimeStamp = Time.time;
            if (cursor == false)
            {
                cursor = true;
                if (inputName.Length < maxStringLength)
                {
                    cursorChar += "_";
                }
            }
            else
            {
                cursor = false;
                if (cursorChar.Length != 0)
                {
                    cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);
                }
            }
        }
    }

    void TrimName()
    {
        if (inputName.Length == 0)    return;
        
        if (inputName.Length > maxStringLength)
            inputName = inputName.Remove(maxStringLength - 1);
    }

    void HandleKeyInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            HandleSubmit();    
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            HandleEscape();
        }
        else if (Input.GetButtonDown("Backspace"))
        {
            HandleDelete();   
        }
        else
        {
            HandleTextInput();
        }
    }

    void HandleTextInput()
    {
        string str = Input.inputString;

        for (int i = 0; i < str.Length; i++)
        {
            int ASCIICode = (int)str[i];

            if (ASCIICode > 32 && ASCIICode <= 126)
            {
                if (inputName.Length >= maxStringLength)
                {
                    // TODO: play error noise, too long
                    return;
                }
                inputName += Input.inputString;
            }
            else
            {
                // TODO: play error noise, out of character range
            }
        }
    }

    void HandleSubmit()
    {
        if (inputName.Length < 1)
        {
            // TODO: play error noise
            return;
        }
        
        TrimName();
        GetComponent<Script_DialogueManager>().EndInputMode(
            new Model_PlayerState(inputName, null, null, null, null)
        );
    }

    void HandleEscape()
    {
        inputName = "";
    }

    void HandleDelete()
    {
        if (inputName.Length == 0)    return;
        inputName = inputName.Remove(inputName.Length - 1);
    }

    // void OnGUI()
    // {   
    //     Event e = Event.current;
        
    //     if (e.isKey)
    //     {
    //         char character = e.character;
    //         int ASCIICode = (int)character;
            
    //         if (ASCIICode >= 32 && ASCIICode <= 126)
    //         {
    //             inputName.text += character;
    //         }
    //     }
    // }
}
