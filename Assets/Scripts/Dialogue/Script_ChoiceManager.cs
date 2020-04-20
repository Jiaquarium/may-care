using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_ChoiceManager : MonoBehaviour
{
    public CanvasGroup choiceCanvas;
    public Script_DialogueChoice[] choices;

    // TODO, mouseclick doesnt deselect the choices
    public void StartChoiceMode(Model_DialogueNode node)
    {
        print("starting choice mode");

        for (int i = 0; i < node.children.Length; i++)
        {
            choices[i].Id = i;
            Text text = Script_Utils.FindComponentInChildWithTag<Text>(
                choices[i].gameObject,
                "tag_dialogue-choice-text"
            );
            text.text = node.children[i].choiceText;
        }

        choiceCanvas.gameObject.SetActive(true);
    }

    public void InputChoice(int Id)
    {
        EndChoiceMode();
        // call dialogue manager to tell it to go to next node
        // dm.currentNode = node.children[Id]

        // TODO: need a way for game to react to your choice
        // game.ChoiceEvent()?
    }

    public void EndChoiceMode()
    {
        choiceCanvas.gameObject.SetActive(false);
    }

    public void Setup()
    {
        choiceCanvas.gameObject.SetActive(false);
    }
}
