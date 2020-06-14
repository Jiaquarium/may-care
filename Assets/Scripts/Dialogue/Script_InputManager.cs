using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script_InputManager : MonoBehaviour
{
    public int ASCIICodeStart;
    public int ASCIICodeEnd;
    public int maxStringLength;
    
    public Script_InputAudioSettings settings;
    public AudioSource audioSource;
    public float errorVol;
    public float typingVol;
    public float deleteVol;
    public TextMeshProUGUI inputDisplay;


    private AudioClip typingSFX;
    private AudioClip errorSFX;
    private AudioClip deleteSFX;
    private string inputName = "";
    private float m_TimeStamp;
    private bool cursor = false;
    private string cursorChar = "";

    void Update()
    {
        Cursor();
        TrimName();
        HandleKeyInput();
        DisplayInputName();
    }

    void DisplayInputName()
    {
        inputDisplay.text = inputName + cursorChar;
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
        if (Input.GetButtonDown(Const_KeyCodes.Submit))
        {
            HandleSubmit();    
        }
        else if (Input.GetButtonDown(Const_KeyCodes.Cancel))
        {
            HandleEscape();
        }
        else if (Input.GetButtonDown(Const_KeyCodes.Backspace))
        {
            HandleDelete();
        }
        else if (Input.anyKeyDown)
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

            if (ASCIICode >= ASCIICodeStart && ASCIICode <= ASCIICodeEnd)
            {
                if (inputName.Length >= maxStringLength)
                {
                    // play error noise, too long of input
                    audioSource.PlayOneShot(errorSFX, errorVol);
                    return;
                }

                audioSource.PlayOneShot(typingSFX, typingVol);
                inputName += str[i];
            }
            else
            {
                audioSource.PlayOneShot(errorSFX, errorVol);
            }
        }
    }

    void HandleSubmit()
    {
        if (inputName.Length < 1)
        {
            audioSource.PlayOneShot(errorSFX, errorVol);
            return;
        }
        
        audioSource.PlayOneShot(typingSFX, typingVol);
        TrimName();
        GetComponent<Script_DialogueManager>().EndInputMode(
            new Model_PlayerState(inputName, null, null, null, null)
        );
    }

    void HandleEscape()
    {
        audioSource.PlayOneShot(deleteSFX, deleteVol);
        
        inputName = "";
    }

    void HandleDelete()
    {
        audioSource.PlayOneShot(deleteSFX, deleteVol);

        if (inputName.Length == 0)    return;
        inputName = inputName.Remove(inputName.Length - 1);
    }

    public void Setup()
    {
        errorSFX = settings.errorSFX;
        typingSFX = settings.typingSFX;
        deleteSFX = settings.deleteSFX;
    }
}
