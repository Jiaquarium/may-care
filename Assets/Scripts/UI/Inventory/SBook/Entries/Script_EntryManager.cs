using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 
/// 1 entry per save point; this way we can keep these in order
/// saving on a previously saved savePoint will OVERWRITE previous
/// </summary>
public class Script_EntryManager : MonoBehaviour
{
    public Script_Game game;
    public Script_Entry entryPrefab;
    public Transform entriesParent;
    public RectTransform scrollContainer;
    public RectTransform maskContainer;
    public Scrollbar scrollbar;
    public Script_EntriesViewController entriesViewController;
    [SerializeField] private int overflowEntriesCount;
    // currently calced as paddingTop + entry...spacing...entry + paddingBot
    // matches MaskContainer height so entries are top aligned
    [SerializeField] private float scrollContainerDefaultHeight;
    // once we start overflowing, begin at this height
    [SerializeField] private float overflowStartingHeight;

    public virtual void OnInspectorGUI(){}

    public void AddEntry(string nameId, string text, string timestamp, string headline)
    {
        Script_Entry existingEntry = GetExistingEntry(nameId);
        if (existingEntry != null)
        {
            EditEntry(existingEntry, text, timestamp);
            return;
        }
        
        // instantiate Entry
        Script_Entry e = Instantiate(entryPrefab, Vector3.zero, Quaternion.identity);
        e.transform.SetParent(entriesParent, false);
        
        int Id = game.entries.Length;
        
        print($"adding headline: {headline}");
        e.Setup(Id, nameId, text, timestamp, headline);
        e.GetComponent<Script_EntryOnSelect>().Setup(entriesViewController);

        // update game refs to entries
        Script_Entry[] newEntries = new Script_Entry[game.entries.Length + 1];
        newEntries = Script_Utils.CopyArrayElements(game.entries, newEntries);
        newEntries[newEntries.Length - 1] = e;
        game.UpdateEntries(newEntries);

        // set explicit navigation and container sizing
        LinkNavigationWithPrevious(Id, entriesParent);
        HandleContainerSizing(e);

        // update EntriesCanvasState if was empty canvas before
        entriesViewController.UpdateCanvasState();
    }

    void LinkNavigationWithPrevious(int Id, Transform parent)
    {
        // if first element return
        if (Id == 0)   return;

        // get ref to last child
        Transform aboveChild = parent.GetChild(Id - 1);
        Transform thisChild = parent.GetChild(Id);

        // set aboveChild selectOnDown nav to thisChild
        Navigation aboveChildNav = aboveChild.GetComponent<Selectable>().navigation;
        aboveChildNav.selectOnDown = thisChild.GetComponent<Button>();
        aboveChild.GetComponent<Selectable>().navigation = aboveChildNav;

        // set thisChild selectOnUp nav to aboveChild
        Navigation thisChildNav = thisChild.GetComponent<Selectable>().navigation;
        thisChildNav.selectOnUp = aboveChild.GetComponent<Button>();
        thisChild.GetComponent<Selectable>().navigation = thisChildNav;
    }

    /// <summary>
    /// 
    /// Add additional height for scrollContainer with each overflowing entry
    /// this will allow the scrollbar to calculate it
    /// </summary>    
    void HandleContainerSizing(Script_Entry e)
    {
        if (game.entries.Length == overflowEntriesCount)
            scrollContainer.sizeDelta = new Vector2(scrollContainer.sizeDelta.x, overflowStartingHeight);
        else if (game.entries.Length > overflowEntriesCount)
            IncreaseScrollContainerHeightByEntryHeight(e);
        else
            InitializeState();
        
        scrollbar.value = 1f;
    }

    void IncreaseScrollContainerHeightByEntryHeight(Script_Entry e)
    {
        scrollContainer.sizeDelta = new Vector2(
            scrollContainer.sizeDelta.x,
            scrollContainer.sizeDelta.y
                + e.GetComponent<RectTransform>().rect.height
                + scrollContainer.GetComponent<VerticalLayoutGroup>().spacing
        );
    }

    void ScrollContainerDefaultHeight()
    {
        scrollContainer.sizeDelta = new Vector2(
            scrollContainer.sizeDelta.x,
            scrollContainerDefaultHeight
        );
    }

    public void EditEntry(Script_Entry e, string newText, string newTimestamp)
    {
        e.Edit(newText, newTimestamp);
    }

    public void ClearEntries()
    {
        game.ClearEntries();
    }

    public Script_Entry GetExistingEntry(string nameId)
    {
        // first check if we've already made this entry
        foreach (Script_Entry gameEntry in game.entries)
        {
            if (nameId == gameEntry.nameId)
            {
                return gameEntry;
            }
        }

        return null;
    }

    public void InitializeState()
    {
        ScrollContainerDefaultHeight();
    }
}


[CustomEditor(typeof(Script_EntryManager))]
public class Script_EntryTester : Editor
{
    /* ==============================================================================
        TEST VALUES
    ============================================================================== */
    private string testNameId = Const_SavePoints.TedwichId;
    private string testText = "this game is so much fun... but a little creepy too!";
    private string testTimestamp = "8:00PM JUN 6 2020";
    private string testHeadline = "hallway encounter with a giant...";
    /* ============================================================================ */
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_EntryManager em = (Script_EntryManager)target;
        if (GUILayout.Button("Add Entry 1"))
        {
            em.AddEntry(testNameId, testText, testTimestamp, testHeadline);
        }
        if (GUILayout.Button("Clear Entries & Set Default"))
        {
            em.ClearEntries();
            em.InitializeState();
        }
    }
}