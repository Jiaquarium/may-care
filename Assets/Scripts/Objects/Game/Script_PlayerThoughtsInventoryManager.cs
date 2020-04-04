using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Script_PlayerThoughtsInventoryManager : MonoBehaviour
{
    public CanvasGroup thoughtsCanvasGroup;
    public Canvas thoughtsCanvas;
    public Canvas emptyStateCanvas;

    public void OpenInventory(
        bool hasThoughts
    )
    {
        print("OpenInventory in ScriptPlayerThoughtsInvManager: " + thoughtsCanvasGroup);
        thoughtsCanvasGroup.alpha = 1f;
        thoughtsCanvasGroup.blocksRaycasts = true;
        
        if (hasThoughts)
        {
            thoughtsCanvas.enabled = true;
        } else
        {
            emptyStateCanvas.enabled = true;
        }
    }

    public void CloseInventory()
    {
        thoughtsCanvas.enabled = false;
        emptyStateCanvas.enabled = false;
        
        thoughtsCanvasGroup.alpha = 0f;
        thoughtsCanvasGroup.blocksRaycasts = false;
    }

    public void AddPlayerThought(
        Model_Thought thought,
        Script_PlayerThoughtsInventoryButton thoughtButton
    )
    {
        thoughtButton.text.text = thought.thought;
    }
}
