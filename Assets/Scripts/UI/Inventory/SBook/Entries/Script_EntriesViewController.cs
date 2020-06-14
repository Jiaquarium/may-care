using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_EntriesViewController : Script_SBookViewController
{
    public Transform noEntriesView;
    public Transform entriesView;
    public Transform entriesHolder;
    public Transform entryDetailNoneSelectedView;
    public Transform entryDetailSelectedView;
    
    public void OnEntrySelect(string text)
    {
        entryDetailSelectedView.GetComponent<Script_EntryDetailView>().SetText(text);
        
        ShowEntryDetail();
    }

    protected override void SetLast()
    {
        base.SetLast();
        lastSlotIndex = lastSelected.GetComponent<Script_Entry>().Id;
    }

    public void UpdateCanvasState()
    {
        UpdateSlots();
        
        if (entriesHolder.childCount == 0)
        {
            noEntriesView.gameObject.SetActive(true);
            entriesView.gameObject.SetActive(false);
        }
        else
        {
            entriesView.gameObject.SetActive(true);
            noEntriesView.gameObject.SetActive(false);
        }
    }

    void HideEntryDetail()
    {
        entryDetailNoneSelectedView.gameObject.SetActive(true);
        entryDetailSelectedView.gameObject.SetActive(false);
    }

    void ShowEntryDetail()
    {
        entryDetailSelectedView.gameObject.SetActive(true);
        entryDetailNoneSelectedView.gameObject.SetActive(false);
    }
    
    void InitializeState()
    {
        UpdateCanvasState();
        HideEntryDetail();
    }
    
    public override void Setup()
    {
        base.Setup();

        InitializeState();
    }
}
