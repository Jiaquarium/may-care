using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_InventoryController : MonoBehaviour
{
    
    public bool isSBookDisabled;
    public GameObject SBookOverviewButton;
    public GameObject initialStateSelected;
    public Script_CanvasGroupController_Thoughts thoughtsController;
    public Script_SBookOverviewController SBookController;
    public Script_StickersViewSettings stickersSettings;
    public Script_EntriesViewSettings entriesSettings;
    public Script_InventoryOverviewSettings inventoryOverviewSettings;
    public string state;
    
    [SerializeField] private Script_InventoryInputManager inputManager;
    [SerializeField] private string newlySelected;
    [SerializeField] private string currentSelected;


    void Update()
    {        
        if (string.IsNullOrEmpty(state))
        {
            InitializeState();
        }
        else if (state == Const_States_InventoryOverview.Overview)
        {
            HandleActiveButton();
            ShowActivePanel();
            inputManager.HandleExitInput();
        }
    }

    public void ChangeStateToOverview()
    {
        state = Const_States_InventoryOverview.Overview;
        ChangeRepeatDelay(inventoryOverviewSettings.repeatDelay, inventoryOverviewSettings.inputActionsPerSecond);
    }

    public void ChangeStateToStickersView()
    {
        state = Const_States_InventoryOverview.StickersView;
        ChangeRepeatDelay(stickersSettings.repeatDelay, stickersSettings.inputActionsPerSecond);
    }

    public void ChangeStateToEntriesView()
    {
        state = Const_States_InventoryOverview.EntriesView;
        ChangeRepeatDelay(entriesSettings.repeatDelay, entriesSettings.inputActionsPerSecond);
    }

    void HandleActiveButton()
    {    
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Script_ButtonMetadata lastSelectedBtn = EventSystem
                .current.currentSelectedGameObject
                .GetComponent<Script_ButtonMetadata>();
            newlySelected = lastSelectedBtn.UIId;
            ChangeRepeatDelay(lastSelectedBtn.repeatDelay, lastSelectedBtn.inputActionsPerSecond);
        }
    }

    void ShowActivePanel()
    {
        // TODO, distinguish between empty state and thoughts canvas update
        // we need to update when moving from empty state thoughts -> thought canvas
        // or will remain on empty state
        // if (currentSelected == newlySelected)    return;
        
        if (newlySelected == Script_UIIds.Thoughts)
        {
            currentSelected = Script_UIIds.Thoughts;
            thoughtsController.Open();
            SBookController.Close();
        }
        else if (newlySelected == Script_UIIds.SBook)
        {
            currentSelected = Script_UIIds.SBook;
            SBookController.Open();
            thoughtsController.Close();
        }
    }

    // allow menu to be "sticker" for non multiple UI
    public void ChangeRepeatDelay(float t, int a)
    {
        EventSystem.current.GetComponent<StandaloneInputModule>().repeatDelay = t;
        EventSystem.current.GetComponent<StandaloneInputModule>().inputActionsPerSecond = a;
    }

    public void EnableSBook(bool isActive)
    {
        SBookOverviewButton.SetActive(isActive);
    }

    void InitializeState()
    {
        EventSystem.current.SetSelectedGameObject(initialStateSelected);
        state = Const_States_InventoryOverview.Overview;
    }

    public void Setup()
    {
        inputManager = GetComponent<Script_InventoryInputManager>();

        inputManager.Setup();
        thoughtsController.Setup();
        SBookController.Setup();

        isSBookDisabled = true;

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("<b>SBook</b> enabled by default for debugging.");
            isSBookDisabled = false;
        }

        if (isSBookDisabled)    EnableSBook(false);
    }
}
