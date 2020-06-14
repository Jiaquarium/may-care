using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SaveManager : MonoBehaviour
{
    public Script_Game game;
    public CanvasGroup saveChoiceCanvas;
    public CanvasGroup saveEntryCanvas;
    public GameObject saveProgressCanvasGroup;
    public GameObject saveProgressCanvas;
    public GameObject saveCompleteCanvas;
    public Script_DialogueChoice[] choices;
    public Script_DialogueManager dm;
    public Script_EntryInput entryInput;
    public Script_EntryManager entryManager;

    
    [SerializeField]
    private float showSavingMinTime;
    [SerializeField]
    private float showSavingCompleteTime;
    private bool isShowingSaving;

    public void StartSavePromptMode()
    {
        // to get rid of flash at beginning
        foreach(Script_DialogueChoice choice in choices)
        {
            choice.cursor.enabled = false;
        }

        saveChoiceCanvas.gameObject.SetActive(true);
    }

    // called on save prompt
    public void InputChoice(int Id)
    {
        // yes, save
        if (Id == 0)
        {
            string savePointNameId = game.GetSavePointData().nameId;
            entryInput.InitializeState(savePointNameId);
            
            StartEntryMode();
            dm.HideDialogue();
        }
        // no, don't save
        else
        {
            dm.NextDialogueNode(1);
        }
        EndSavePrompt();
    }

    // called on save entry view
    public void InputSaveEntryChoice(int Id, string playerInputText)
    {
        // Submit
        if (Id == 0)
        {
            Model_SavePointData spData = game.GetSavePointData();
            
            // create Entry
            entryManager.AddEntry(
                spData.nameId,
                playerInputText,
                "TIMESTAMP TBD",     // TBD
                spData.headline
            );
            
            saveProgressCanvas.SetActive(true);
            Script_SaveGameControl.control.Save();

            isShowingSaving = true;
            Script_AwaitFile.AwaitFile(Script_SaveGameControl.path);
            isShowingSaving = false;
            
            StartCoroutine(AwaitSaveComplete());
        }
        // Cancel
        else if (Id == 1)
        {
            dm.NextDialogueNode(1);
        }
        EndEntryMode();
    }

    /// <summary>
    ///     to mock saving; our saves are near-instant but i still want the feel of a "real" save
    /// </summary>
    IEnumerator AwaitSaveComplete()
    {
        yield return new WaitForSeconds(showSavingMinTime);
        // wait for AwaitFile if needed (it will turn isShowingSaving false)
        while (isShowingSaving)    { }

        saveCompleteCanvas.SetActive(true);
        saveProgressCanvas.SetActive(false);

        yield return new WaitForSeconds(showSavingCompleteTime);

        saveCompleteCanvas.SetActive(false);
        dm.NextDialogueNode(0);
    }

    void EndSavePrompt()
    {
        saveChoiceCanvas.gameObject.SetActive(false);
    }

    void StartEntryMode()
    {
        saveEntryCanvas.gameObject.SetActive(true);
    }

    void EndEntryMode()
    {
        saveEntryCanvas.gameObject.SetActive(false);
    }

    public void Setup()
    {
        saveChoiceCanvas.gameObject.SetActive(false);
        saveEntryCanvas.gameObject.SetActive(false);
        saveProgressCanvasGroup.SetActive(true);
        saveProgressCanvas.SetActive(false);
        saveCompleteCanvas.SetActive(false);
        entryInput.Setup();
    }
}
