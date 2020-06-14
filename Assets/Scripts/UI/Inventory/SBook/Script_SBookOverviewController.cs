using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Script_SBookOverviewController : Script_CanvasGroupController
{
    public GameObject SBookOutsideCanvas;
    public GameObject SBookInsideCanvas;
    public GameObject insideSBookInitialStateSelected;
    public GameObject outsideSBookInitialStateSelected;
    public GameObject topBarButtonToChangeNav;
    public GameObject insideSBookSelectOnDownFromTopBar;
    public GameObject outsideSBookSelectOnDownFromTopBar;
    public GameObject[] insideSBookBtnsToTrack;
    public GameObject[] outsideSBookBtnsToTrack;
    public Script_StickersViewController stickersViewController;
    public Script_InventoryController inventoryController;
    public Script_EntriesViewController entriesViewController;

    [SerializeField] private bool isInsideSBook;
    [SerializeField] private GameObject lastSelectedBeforeExit;

    
    private void Update() {
        if (inventoryController.state != Const_States_InventoryOverview.Overview)   return;
        
        if (isInsideSBook)
        {
            SetInsideLastSelected();
        }
        else
        {
            SetOutsideLastSelected();
        }
    }

    public void EnterStickersView()
    {
        stickersViewController.gameObject.SetActive(true);
        stickersViewController.RehydrateState();

        inventoryController.ChangeStateToStickersView();
    }

    public void ExitStickerView()
    {
        inventoryController.ChangeStateToOverview();
        stickersViewController.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(lastSelectedBeforeExit);
    }

    public void EnterEntriesView()
    {
        print("attempting to enter entries");
        entriesViewController.gameObject.SetActive(true);
        entriesViewController.RehydrateState();
        
        inventoryController.ChangeStateToEntriesView();
    }

    public void ExitEntriesView()
    {
        inventoryController.ChangeStateToOverview();
        entriesViewController.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(lastSelectedBeforeExit);
    }
    
    public void EnterSBook()
    {
        SetOverviewNavigationInsideSBook();
        
        SBookInsideCanvas.SetActive(true);
        SBookOutsideCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(insideSBookInitialStateSelected);
        isInsideSBook = true;
    }

    public void ExitSBook()
    {
        SetOverviewNavigationOutsideSBook();

        SBookInsideCanvas.SetActive(false);
        SBookOutsideCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(outsideSBookInitialStateSelected);
        isInsideSBook = false;
    }

    void SetOverviewNavigationInsideSBook()
    {
        Navigation btnNav = topBarButtonToChangeNav.GetComponent<Selectable>().navigation;
        btnNav.selectOnDown = insideSBookSelectOnDownFromTopBar.GetComponent<Button>();
        topBarButtonToChangeNav.GetComponent<Selectable>().navigation = btnNav;
    }

    void SetOverviewNavigationOutsideSBook()
    {
        Navigation btnNav = topBarButtonToChangeNav.GetComponent<Selectable>().navigation;
        btnNav.selectOnDown = outsideSBookSelectOnDownFromTopBar.GetComponent<Button>();
        topBarButtonToChangeNav.GetComponent<Selectable>().navigation = btnNav;
    }

    void SetInsideLastSelected()
    {
        foreach (GameObject b in insideSBookBtnsToTrack)
        {
            if (
                EventSystem.current.currentSelectedGameObject == b
                && insideSBookSelectOnDownFromTopBar != b
            )
            {
                lastSelectedBeforeExit = b;
                insideSBookSelectOnDownFromTopBar = b;
                SetOverviewNavigationInsideSBook();
            }
        }
    }

    void SetOutsideLastSelected()
    {
        foreach (GameObject b in outsideSBookBtnsToTrack)
        {
            if (
                EventSystem.current.currentSelectedGameObject == b
                && outsideSBookSelectOnDownFromTopBar != b
            )
            {
                lastSelectedBeforeExit = b;
                outsideSBookSelectOnDownFromTopBar = b;
                SetOverviewNavigationOutsideSBook();
            }
        }
    }

    void InitializeState()
    {
        SetOverviewNavigationOutsideSBook();

        SBookInsideCanvas.SetActive(false);
        SBookOutsideCanvas.SetActive(true);
        isInsideSBook = false;
        stickersViewController.gameObject.SetActive(false);
        entriesViewController.gameObject.SetActive(false);
    }

    public override void Setup()
    {
        InitializeState();
        stickersViewController.Setup();
        entriesViewController.Setup();
    }
}
