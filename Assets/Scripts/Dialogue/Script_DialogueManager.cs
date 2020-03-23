using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_DialogueManager : MonoBehaviour
{
    public Script_Game game;
    private Script_InputManager inputManager;
    public CanvasGroup inputManagerCanvas;
    public CanvasGroup canvas;
    public Text nameText;
    public Text dialogueText;
    private Script_Player player;
    private string playerName;
    public bool isRenderingSentence = false;
    public AudioSource audioSource;
    public AudioClip dialogueStartSoundFX;
    public Queue<string> sentences;
    private IEnumerator coroutine;
    private string formattedSentence;
    private bool isInputMode = false;

    public void StartDialogue(Script_Dialogue dialogue)
    {
        nameText.text = dialogue.name != null ? dialogue.name + ":" : "";
        
        player.SetIsTalking();
        ShowDialogue();
        sentences.Clear();

        audioSource.PlayOneShot(dialogueStartSoundFX, 1.0f);

        foreach(string _sentence in dialogue.sentences)
        {
            sentences.Enqueue(_sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {   
        playerName = game.GetPlayerState().name;
        
        // prevent from stacking continuations
        if (isRenderingSentence || isInputMode)    return;

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string unformattedSentence = sentences.Dequeue();

        formattedSentence = string.Format(unformattedSentence, playerName);

        coroutine = TypeSentence(formattedSentence);
        
        StartCoroutine(coroutine);
    }

    IEnumerator TypeSentence(string sentence)
    {
        StartRenderingSentence();

        dialogueText.text = "";
        foreach (char letter in formattedSentence.ToCharArray())
        {
            if (letter.Equals('|'))
            {
                 yield return new WaitForSeconds(0.25f);
            }
            else
            {
                dialogueText.text += letter;
                yield return null;
            }
        }

        FinishRenderingSentence();
    }

    void StartRenderingSentence()
    {
        isRenderingSentence = true;
    }
    
    void FinishRenderingSentence()
    {
        isRenderingSentence = false;

        // check next sentence for custom command
        CheckCustomCommand();
    }

    bool CheckCustomCommand()
    {   
        if (sentences.Count != 0 && sentences.Peek() == "<INPUT>")
        {
            sentences.Dequeue();
            StartInputMode();

            return true;
        }

        return false;
    }

    public void StartInputMode()
    {
        isInputMode = true;

        // set input canvas active
        inputManagerCanvas.gameObject.SetActive(true);
        inputManager.enabled = true;
    }

    public void EndInputMode(Dictionary<string, string> state)
    {
        isInputMode = false;

        game.SetPlayerState(state);

        // set input canvas active
        inputManagerCanvas.gameObject.SetActive(false);
        inputManager.enabled = false;

        DisplayNextSentence();
    }

    public void EndDialogue()
    {
        HideDialogue();
        player.SetIsNotTalking();
    }

    public void SkipTypingSentence()
    {
        if (isRenderingSentence)
        {
            StopCoroutine(coroutine);
            
            formattedSentence = formattedSentence.Replace("|", string.Empty);
            
            dialogueText.text = formattedSentence;
            FinishRenderingSentence();
        }
    }

    public void HideDialogue()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
    }

    void ShowDialogue()
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
    }

    public void Setup()
    {
        player = FindObjectOfType<Script_Player>();
        sentences = new Queue<string>();
        
        inputManager = GetComponent<Script_InputManager>();
        inputManager.enabled = false;
        inputManagerCanvas.gameObject.SetActive(false);
    }
}
