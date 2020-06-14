using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Script_PlayerThoughtsInventoryManager : MonoBehaviour
{
    public CanvasGroup inventoryCanvasGroup;
    public CanvasGroup thoughtsCanvasGroup;
    public CanvasGroup sBookCanvasGroup;
    public Script_InventoryController inventoryController;
    public Script_ThoughtSlotHolder thoughtSlotHolder;

    [SerializeField]
    private Script_Game game;

    public void OpenInventory()
    {
        inventoryCanvasGroup.gameObject.SetActive(true);
        inventoryCanvasGroup.alpha = 1f;
        inventoryCanvasGroup.blocksRaycasts = true;
    }

    public void CloseInventory()
    {
        inventoryCanvasGroup.gameObject.SetActive(false);
        inventoryCanvasGroup.alpha = 0f;
        inventoryCanvasGroup.blocksRaycasts = false;   
    }

    public void EnableSBook(bool isActive)
    {
        inventoryController.EnableSBook(isActive);
    }

    public void AddPlayerThought(
        Model_Thought thought,
        Script_PlayerThoughtsInventoryButton thoughtButton
    )
    {
        thoughtButton.text.text = thought.thought;
    }

    public void InitializeState()
    {
        CloseInventory();
        thoughtsCanvasGroup.gameObject.SetActive(false);
        sBookCanvasGroup.gameObject.SetActive(false);
    }

    public void Setup()
    {
        inventoryController.Setup();
        
        // setup number of slots
        Transform slotHolder = thoughtSlotHolder.transform;
        Script_PlayerThoughtsInventoryButton[] thoughtSlots = new Script_PlayerThoughtsInventoryButton[
            slotHolder.childCount
        ];
        for (int i = 0; i < thoughtSlots.Length; i++)
        {
            thoughtSlots[i] = slotHolder.GetChild(i)
                .GetComponent<Script_PlayerThoughtsInventoryButton>();
        }
        game = transform.parent.GetComponent<Script_Game>();
        game.thoughtSlots = thoughtSlots; 

        InitializeState();
    }
}
