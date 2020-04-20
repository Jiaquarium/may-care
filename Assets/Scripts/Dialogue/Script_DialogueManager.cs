using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Script_DialogueManager : MonoBehaviour
{
    public CanvasGroup inputManagerCanvas;
    public Script_Game game;
    public CanvasGroup canvas;
    public AudioSource audioSource;
    public AudioClip dialogueStartSoundFX;
    public AudioClip typeSFX;

    
    public Text activeCanvasText;
    public Text[] activeCanvasTexts;
    /*
        NAME
        LINE
        LINE
    */
    public Canvas DefaultCanvas;
    public Text DefaultCanvasName;
    public Text[] DefaultCanvasDialogueTexts;

    /*
        LINE
        LINE
    */
    public Canvas DefaultReadTextCanvas;
    public Text[] DefaultReadTextCanvasTexts;

    

    public bool isRenderingDialogueSection = false;
    public bool isRenderingLine = false;
    public int lineCount = 0;
    public Queue<Model_DialogueSection> dialogueSections;
    public Queue<string> lines;
    public float pauseLength;
    public float charPauseLength;
    public float typingVolumeScale;
    public float dialogueStartVolumeScale;
    public Model_DialogueNode currentNode;

    
    private Text nameText;
    private Text dialogueText;
    private Script_InputManager inputManager;
    private Script_ChoiceManager choiceManager;
    private Script_Player player;
    private string playerName;
    private IEnumerator coroutine;
    private Model_DialogueSection dialogueSection;
    private string formattedLine;
    private bool isInputMode = false;
    private bool shouldPlayTypeSFX = true;
    private bool isSilentTyping = false;
    private bool isDialogueTreeActive = false;

    public void StartDialogueNode(Model_DialogueNode node)
    {
        // look at node.dialogue
        isDialogueTreeActive = true;
        currentNode = node;
        // startdialogue with node.dialogue
        StartDialogue(currentNode.dialogue, null);
    }

    bool NextDialogueNode()
    {
        // check if node has children
        if (currentNode.children.Length > 1)
        {
            print("go to choices");
            // show choices canvas
            isInputMode = true;
            choiceManager.StartChoiceMode(currentNode);

            return true;
        }

        // check next if null then stop
        // if node has one branch
        else if (currentNode.children.Length == 1)
        {
            print("go to next node");
            // currentnode is next
            currentNode = currentNode.children[0];
            // start dialogue with new node
            StartDialogue(currentNode.dialogue, null);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartDialogue(Model_Dialogue dialogue, string type)
    {
        if (type == "read")
        {
            DefaultReadTextCanvas.enabled = true;
            DefaultCanvas.enabled = false;

            activeCanvasTexts = DefaultReadTextCanvasTexts;
        }
        else
        {
            DefaultCanvas.enabled = true;
            DefaultReadTextCanvas.enabled = false;
            activeCanvasTexts = DefaultCanvasDialogueTexts;

            nameText = DefaultCanvasName;
            nameText.text = dialogue.name + ":";

            if (Debug.isDebugBuild && (dialogue.name == "" || dialogue.name == null))
            {
                Debug.Log("No name was provided for dialogue");
            }
        }
        
        if (type == "read")    isSilentTyping = true;
        else                   isSilentTyping = false;

        ClearState();
        player.SetIsTalking();
        ShowDialogue();

        audioSource.PlayOneShot(dialogueStartSoundFX, dialogueStartVolumeScale);

        foreach(Model_DialogueSection _dialogueSection in dialogue.sections)
        {
            dialogueSections.Enqueue(_dialogueSection);
        }

        DisplayNextDialoguePortion();
    }

    public void DisplayNextDialoguePortion()
    {   
        playerName = game.GetPlayerState().name;
        
        // prevent from stacking continuations
        if (isRenderingDialogueSection || isInputMode)    return;

        if (dialogueSections.Count == 0)
        {
            EndDialogue();
            return;
        }

        StartRenderingDialoguePortion();

        dialogueSection = dialogueSections.Dequeue();

        foreach(string _line in dialogueSection.lines)
        {
            lines.Enqueue(_line);
        }
        
        lineCount = 0;
        ClearTextCanvases();
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        // prevent from stacking continuations
        // if (isRenderingDialogueSection || isInputMode)    return;
        if (lines.Count == 0)
        {
            FinishRenderingDialogueSection();
            return;
        }
        activeCanvasText = activeCanvasTexts[lineCount];
        
        string unformattedLine = lines.Dequeue();

        formattedLine = string.Format(unformattedLine, playerName);

        coroutine = TypeLine(formattedLine);
        
        StartCoroutine(coroutine);
    }

    IEnumerator TypeLine(string sentence)
    {
        StartRenderingLine();

        bool isTracking = false;
        bool isWrapNextLetter = false;
        bool isFindingClosingTags = false;
        bool isSkipLetter = false;
        
        string wrappedChar = "";
        string wrap = "";
        
        int tagsCount = 0;

        activeCanvasText.text = "";
        
        foreach(char letter in formattedLine.ToCharArray())
        {
            // play TypeSFX on pauses
            if (letter.Equals('|'))
            {
                shouldPlayTypeSFX = true;
                yield return new WaitForSeconds(pauseLength);
            }

            /*
                start: tag converter algo, takes tagged text
                e.g. <i><b><size=18>hello world</i></b></size=18> into
                <i><b><size=18>h</i></b></size=18><i><b><size=18>e</i></b></size=18> etc etc...
            */
            else if (isFindingClosingTags)
            {
                if (letter.Equals('>'))
                {
                    tagsCount--;
                    // reset state, we know we've covered all the tags we've added
                    if (tagsCount == 0)
                    {
                        // sets to starting state
                        wrappedChar = "";
                        wrap = ""; 

                        isFindingClosingTags = false;
                        isTracking = false;
                        isSkipLetter = false;
                    }
                }
            }
            else
            {
                if (isWrapNextLetter)
                {
                    // if we find < and no wrapped Char yet, we know it's another tag
                    if (letter.Equals('<'))
                    {
                        if (wrappedChar == "")
                        {
                            wrap += letter;
                            isSkipLetter = true;
                            isWrapNextLetter = false;
                        }
                        // else, done wrapping characters since wrappedChar
                        // is loaded; we know we're now looking for closing tags
                        else
                        {
                            isFindingClosingTags = true;
                            isSkipLetter = true;
                            isWrapNextLetter = false;
                        }
                    }
                    else
                    {
                        // will give you all the tags
                        // ex: <size=18><b><i>[char]</i></b></size>
                        wrappedChar = WrapCharWithTags(wrap, letter);
                        isSkipLetter = false;
                    }
                }
                else if (letter.Equals('<'))
                {
                    isSkipLetter = true;
                    wrap += letter;
                    isTracking = true;
                }
                else if (isTracking)
                {
                    isSkipLetter = true;
                    wrap += letter;

                    if (letter.Equals('>'))
                    {
                        isWrapNextLetter = true;
                        tagsCount++;
                    }
                }

                /*
                    end
                */

                if (!isSkipLetter)
                {
                    // only play typeSFX every other char
                    if (shouldPlayTypeSFX == true && !isSilentTyping)
                    {
                        audioSource.PlayOneShot(typeSFX, typingVolumeScale);
                        shouldPlayTypeSFX = false;
                    } else
                    {
                        shouldPlayTypeSFX = true;
                    }

                    activeCanvasText.text += wrappedChar == "" ? letter.ToString() : wrappedChar;

                    yield return new WaitForSeconds(charPauseLength);
                }
            }
        }

        FinishRenderingLine();
        lineCount++;
        DisplayNextLine();
    }

    string WrapCharWithTags(string wrap, char c)
    {
        // wrap will be <size><i><b>
            // need to check if next char is <, if not we know it's a char
            // if is continue adding to wrap
        // only handles <size...>, <i> & <b>
		
        string size20Rx =           "<size=20";
        string boldRx =             "<b>";
        string italicRx =           "<i>";
        string wrappedChar =        c.ToString();
		
		if (Regex.IsMatch(wrap, size20Rx))  wrappedChar = "<size=20>" + wrappedChar + "</size>";
        if (Regex.IsMatch(wrap, boldRx))    wrappedChar = "<b>" + wrappedChar + "</b>";
        if (Regex.IsMatch(wrap, italicRx))  wrappedChar = "<i>" + wrappedChar + "</i>";

        return wrappedChar;
    }

    // TODO: do we need?
    void StartRenderingLine()
    {
        isRenderingLine = true;
    }

    void FinishRenderingLine()
    {
        isRenderingLine = false;
    }

    void StartRenderingDialoguePortion()
    {
        isRenderingDialogueSection = true;
    }
    
    void FinishRenderingDialogueSection()
    {
        isRenderingDialogueSection = false;

        // check next dialoguePortion for custom command if this is last line
        CheckCustomCommand();
    }

    // TODO: check first line
    bool CheckCustomCommand()
    {   
        if (
            dialogueSections.Count != 0
            && dialogueSections.Peek().lines[0] == "<INPUT>"
        )
        {
            dialogueSections.Dequeue();
            StartInputMode();

            return true;
        }

        return false;
    }

    void ClearState()
    {
        dialogueSections.Clear();
        lines.Clear();
        ClearTextCanvases();
    }

    void ClearTextCanvases()
    {
        foreach (Text t in activeCanvasTexts)
        {
            t.text = "";
        }
    }

    public void StartInputMode()
    {
        isInputMode = true;

        // set input canvas active
        inputManagerCanvas.gameObject.SetActive(true);
        inputManager.enabled = true;
    }

    public void EndInputMode(Model_PlayerState state)
    {
        isInputMode = false;

        game.SetPlayerState(state);

        // set input canvas active
        inputManagerCanvas.gameObject.SetActive(false);
        inputManager.enabled = false;

        DisplayNextDialoguePortion();
    }

    public void EndDialogue()
    {
        if (isDialogueTreeActive)
        {
            if (NextDialogueNode())
            {
                // go to next node or choices and don't end dialogue just yet
                return;
            }
        }
        
        HideDialogue();
        player.SetIsNotTalking();
    }

    public void SkipTypingSentence()
    {
        if (dialogueSection.isUnskippable)  return;

        // replace all dialogue portions
        if (isRenderingDialogueSection)
        {
            StopCoroutine(coroutine);

            for (int i = 0; i < dialogueSection.lines.Length; i++)
            {
                // interpolate playerName
                string unformattedLine = dialogueSection.lines[i];
                string _formattedLine = string.Format(unformattedLine, playerName);
                
                // remove pause indicators
                _formattedLine = _formattedLine.Replace("|", string.Empty);
                
                activeCanvasText = activeCanvasTexts[i];
                activeCanvasText.text = _formattedLine;
            }

            lines.Clear();
            FinishRenderingDialogueSection();
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
        dialogueSections = new Queue<Model_DialogueSection>();
        lines = new Queue<string>();

        inputManager = GetComponent<Script_InputManager>();
        inputManager.enabled = false;
        inputManagerCanvas.gameObject.SetActive(false);

        choiceManager = GetComponent<Script_ChoiceManager>();
        choiceManager.Setup();

        HideDialogue();
    }
}
