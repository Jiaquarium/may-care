using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class Script_DialogueManager : MonoBehaviour
{
    public CanvasGroup inputManagerCanvas;
    public Script_Game game;
    public CanvasGroup canvas;
    public AudioSource audioSource;
    public AudioClip dialogueStartSoundFX;
    public AudioClip typeSFX;

    public Transform activeCanvas;
    public TextMeshProUGUI activeCanvasText;
    public TextMeshProUGUI[] activeCanvasTexts;
    /*
        NAME
        LINE
        LINE
    */
    public Canvas DefaultCanvas;
    public TextMeshProUGUI DefaultCanvasName;
    public TextMeshProUGUI[] DefaultCanvasDialogueTexts;

    public Canvas DefaultCanvasTop;
    public TextMeshProUGUI DefaultCanvasNameTop;
    public TextMeshProUGUI[] DefaultCanvasDialogueTextsTop;

    public Canvas CanvasChoice1Row;
    public TextMeshProUGUI CanvasChoice1RowName;
    public TextMeshProUGUI[] CanvasChoice1RowDialogueTexts;

    public Canvas CanvasChoice1RowTop;
    public TextMeshProUGUI CanvasChoice1RowTopName;
    public TextMeshProUGUI[] CanvasChoice1RowTopDialogueTexts;

    public Canvas SaveChoiceCanvas;
    public TextMeshProUGUI SaveChoiceName;
    public TextMeshProUGUI[] SaveChoiceDialogueTexts;

    /*
        LINE
        LINE
    */
    public Canvas DefaultReadTextCanvas;
    public TextMeshProUGUI[] DefaultReadTextCanvasTexts;
    public Script_SaveManager saveManager;

    

    public bool isRenderingDialogueSection = false;
    public bool isRenderingLine = false;
    public int lineCount = 0;
    public Queue<Model_DialogueSection> dialogueSections;
    public Queue<string> lines;
    public float pauseLength;
    public float charPauseLength;
    public float typingVolumeScale;
    public float dialogueStartVolumeScale;
    public Script_DialogueNode currentNode;
    public bool isInputMode = false;
    public bool noContinuationIcon;
    public bool isKeepingDialogueUp;

    
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private Script_InputManager inputManager;
    private Script_ChoiceManager choiceManager;
    private Script_Player player;
    private string playerName;
    private IEnumerator coroutine;
    private Model_DialogueSection dialogueSection;
    private string formattedLine;
    private bool shouldPlayTypeSFX = true;
    private bool isSilentTyping = false;

    void Update()
    {
        // called after finishes rendering dialogue sections and lines 
        if (
            currentNode != null
            && !string.IsNullOrEmpty(currentNode.data.updateAction)
            && dialogueSections.Count == 0
            && lines.Count == 0
            && !isRenderingDialogueSection
        )
        {
            game.HandleDialogueNodeUpdateAction(currentNode.data.updateAction);
        }
    }
    
    public void StartDialogueNode(
        Script_DialogueNode node,
        bool SFXOn = true,
        string type = null
    )
    {
        // look at node.dialogue
        currentNode = node;

        // startdialogue with node.dialogue
        StartDialogue(currentNode.data.dialogue, type, SFXOn);
    }

    bool CheckNodeChildren()
    {
        if (currentNode.data.children.Length > 0)   return true;
        else                                        return false;
    }

    bool CheckChoices()
    {
        if (currentNode.data.children.Length > 1 && !(currentNode is Script_DialogueNode_SavePoint))
                    return true;
        else        return false;
    }

    void HandleChoices()
    {
        isInputMode = true;
        choiceManager.StartChoiceMode(currentNode);
    }

    public void NextDialogueNode(int i)
    {
        ShowDialogue();
        EndChoiceMode();
        EndSaveEntryMode();
        
        currentNode = currentNode.data.children[i];
        EnqueueDialogueSections(currentNode.data.dialogue);
        SetupCanvases(currentNode.data.dialogue, currentNode.data.type);
        
        nameText.text = FormatString(currentNode.data.dialogue.name) + ":";
        
        DisplayNextDialoguePortion();
    }

    void EnqueueDialogueSections(Model_Dialogue dialogue)
    {
        foreach(Model_DialogueSection _dialogueSection in dialogue.sections)
        {
            dialogueSections.Enqueue(_dialogueSection);
        }
    }
    
    void StartDialogue(
        Model_Dialogue dialogue,
        string type,
        bool SFXOn = true
    )
    {
        isKeepingDialogueUp = false;
        ClearState();
        SetupCanvases(dialogue, type);
        
        if (type == "read")    isSilentTyping = true;
        else                   isSilentTyping = false;

        // TODO: clean this up, this is not always true, especially when it's just
        // extra text
        player.SetIsTalking();
        ShowDialogue();

        if (SFXOn)
        {
            audioSource.PlayOneShot(dialogueStartSoundFX, dialogueStartVolumeScale);
        }

        EnqueueDialogueSections(dialogue);

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
        
        if (dialogueSection.noContinuationIcon)     noContinuationIcon = true;
        else                                        noContinuationIcon = false;

        foreach(string _line in dialogueSection.lines)
        {
            lines.Enqueue(_line);
        }
        
        lineCount = 0;
        ClearActiveCanvasTexts();
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

        if (currentNode.data.isDynamicLines)
        {
            activeCanvasText.enableWordWrapping = false;
            activeCanvasText.overflowMode = TMPro.TextOverflowModes.Overflow;
        }
        else
        {
            activeCanvasText.enableWordWrapping = true;
        }
        
        string unformattedLine = lines.Dequeue();

        formattedLine = FormatString(unformattedLine);

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

        // show choices after finishing typing line or skipped
        if (CheckNodeChildren())
        {
            // check for choices first
            if (CheckChoices())  {
                HandleChoices();
                return;
            }
        }

        if (
            currentNode is Script_DialogueNode_SavePoint
        )
        {
            // the SavePoint node contains the custom prompt
            // after pressing space on a SavePoint node, will prompt the saveChoices
            saveManager.StartSavePromptMode();
            isInputMode = true;
            // saveManager.StartSavePromptMode();
            return;
        }
    }

    bool CheckCustomCommand()
    {   
        if (
            dialogueSections != null
            && dialogueSections.Count != 0
            && dialogueSections.Peek().lines != null
            && dialogueSections.Peek().lines.Length != 0
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
        ClearAllCanvasTexts();
    }

    void ClearActiveCanvasTexts()
    {
        foreach (TextMeshProUGUI t in activeCanvasTexts)
        {
            t.text = "";
        }   
    }

    void ClearAllCanvasTexts()
    {
        DefaultCanvasName.text = "";
        foreach (TextMeshProUGUI t in DefaultCanvasDialogueTexts)
        {
            t.text = "";
        }
        DefaultCanvasNameTop.text = "";
        foreach (TextMeshProUGUI t in DefaultCanvasDialogueTextsTop)
        {
            t.text = "";
        }
        foreach (TextMeshProUGUI t in DefaultReadTextCanvasTexts)
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

    public void EndChoiceMode()
    {
        isInputMode = false;    
    }

    public void EndSaveEntryMode()
    {
        isInputMode = false;
    }

    public void EndDialogue()
    {
        // actions will be activated after "space" is pressed to move to next dialogue
        if (!string.IsNullOrEmpty(currentNode.data.action))
        {
            game.HandleDialogueNodeAction(currentNode.data.action);
        }
        
        if (CheckNodeChildren() && !CheckChoices())
        {
            // we know there is only one next node on continuation
            NextDialogueNode(0);
            return;
        }

        player.SetIsInteract();
        
        // option to keep dialogue up (for command prompts e.g. tutorials)
        if (
            currentNode.data.showDialogueOnAction
            && (
                !string.IsNullOrEmpty(currentNode.data.action)
                || !string.IsNullOrEmpty(currentNode.data.updateAction)
            )
        )
        {
            isKeepingDialogueUp = true;
            return;
        }
        HideDialogue();
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
                string _formattedLine = FormatString(unformattedLine);
                
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
        canvas.gameObject.SetActive(false);
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
    }

    void ShowDialogue()
    {
        canvas.gameObject.SetActive(true);
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
    }

    void SetupCanvases(Model_Dialogue dialogue, string type)
    {
        DefaultCanvas.enabled = false;
        DefaultCanvasTop.enabled = false;
        DefaultReadTextCanvas.enabled = false;
        CanvasChoice1Row.enabled = false;
        CanvasChoice1RowTop.enabled = false;
        SaveChoiceCanvas.enabled = false;
        
        string canvasLocType = "bottom";
        if (currentNode.data.locationType != null)
        {
            canvasLocType = currentNode.data.locationType;
        }
        
        if (type == "read")
        {
            activeCanvas = DefaultReadTextCanvas.transform;
            activeCanvas.GetComponent<Script_Canvas>().ContinuationIcon.Setup();

            DefaultReadTextCanvas.enabled = true;
            activeCanvasTexts = DefaultReadTextCanvasTexts;
        }
        else if (currentNode is Script_DialogueNode_SavePoint)
        {
            activeCanvas = SaveChoiceCanvas.transform;
            SaveChoiceCanvas.enabled = true;
            nameText = SaveChoiceName;
            activeCanvasTexts = SaveChoiceDialogueTexts;
            SetupName(dialogue.name);
        }
        else if (CheckChoices() && !(currentNode is Script_DialogueNode_SavePoint))
        {
            // don't set continuation for choice canvas section
            SetDialogueCanvasToCanvasChoice1Row(canvasLocType);
        }
        else
        {
            if (canvasLocType == "top")
            {
                activeCanvas = DefaultCanvasTop.transform;
                activeCanvas.GetComponent<Script_Canvas>().ContinuationIcon.Setup();

                DefaultCanvasTop.enabled = true;
                activeCanvasTexts = DefaultCanvasDialogueTextsTop;
                nameText = DefaultCanvasNameTop;
            }
            else
            {
                activeCanvas = DefaultCanvas.transform;
                activeCanvas.GetComponent<Script_Canvas>().ContinuationIcon.Setup();

                DefaultCanvas.enabled = true;
                activeCanvasTexts = DefaultCanvasDialogueTexts;
                nameText = DefaultCanvasName;
            }
            SetupName(dialogue.name);           
        }
    }

    void SetupName(string name)
    {
        nameText.text = FormatString(name) + (string.IsNullOrEmpty(name) ? "" : ":");

        if (Debug.isDebugBuild && Const_Dev.IsDevMode && (name == "" || name == null))
        {
            Debug.Log("No name was provided for dialogue");
        }
    }

    void SetDialogueCanvasToCanvasChoice1Row(string loc)
    {
        if  (loc == "top")
        {
            activeCanvas = CanvasChoice1RowTop.transform;
            CanvasChoice1RowTop.enabled = true;
            
            activeCanvasTexts = CanvasChoice1RowTopDialogueTexts;
            nameText = CanvasChoice1RowTopName;
            nameText.text = FormatString(currentNode.data.dialogue.name) + ":";
        }
        else
        {
            activeCanvas = CanvasChoice1Row.transform;
            CanvasChoice1Row.enabled = true;
            
            activeCanvasTexts = CanvasChoice1RowDialogueTexts;
            nameText = CanvasChoice1RowName;
            nameText.text = FormatString(currentNode.data.dialogue.name) + ":";
        }
    }

    string FormatString(string unformattedString)
    {
        return string.Format(
            unformattedString,
            playerName,
            Script_Names.Melz,
            Script_Names.MelzTheGreat,
            Script_Names.Ids,
            Script_Names.Ero,
            Script_Names.SBook
        );
    }

    public void Setup()
    {
        player = FindObjectOfType<Script_Player>();
        dialogueSections = new Queue<Model_DialogueSection>();
        lines = new Queue<string>();

        inputManager = GetComponent<Script_InputManager>();
        inputManager.Setup();
        inputManager.enabled = false;
        inputManagerCanvas.gameObject.SetActive(false);

        choiceManager = GetComponent<Script_ChoiceManager>();
        choiceManager.Setup();

        saveManager.Setup();

        HideDialogue();
    }
}
