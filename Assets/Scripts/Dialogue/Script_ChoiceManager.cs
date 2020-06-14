using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script_ChoiceManager : MonoBehaviour
{
    public CanvasGroup choiceCanvasTop;
    public Script_DialogueChoice[] choicesTop;
    public CanvasGroup choiceCanvasBottom;
    public Script_DialogueChoice[] choicesBottom;
    public CanvasGroup activeCanvas;
    public Script_DialogueChoice[] activeChoices;

    
    private Script_DialogueManager dialogueManager;

    public void StartChoiceMode(Script_DialogueNode node)
    {
        if (node.data.locationType == "top")
        {
            activeCanvas = choiceCanvasTop;
            activeChoices = choicesTop;
        }
        else
        {
            activeCanvas = choiceCanvasBottom;
            activeChoices = choicesBottom;
        }

        // to get rid of flash at beginning
        foreach(Script_DialogueChoice choice in activeChoices)
        {
            choice.cursor.enabled = false;
        }

        for (int i = 0; i < node.data.children.Length; i++)
        {
            activeChoices[i].Id = i;
            TextMeshProUGUI text = Script_Utils.FindComponentInChildWithTag<TextMeshProUGUI>(
                activeChoices[i].gameObject,
                Const_Tags.DialogueChoice
            );
            text.text = node.data.children[i].data.choiceText;
        }

        activeCanvas.gameObject.SetActive(true);
    }

    public void InputChoice(int Id)
    {
        EndChoiceMode();
        dialogueManager.NextDialogueNode(Id);
    }

    void EndChoiceMode()
    {
        activeCanvas.gameObject.SetActive(false);
    }

    public void Setup()
    {
        choiceCanvasTop.gameObject.SetActive(false);
        choiceCanvasBottom.gameObject.SetActive(false);
        dialogueManager = GetComponent<Script_DialogueManager>();
    }
}
