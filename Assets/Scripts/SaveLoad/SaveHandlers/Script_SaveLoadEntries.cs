using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SaveLoadEntries : MonoBehaviour
{
    [SerializeField] private Script_Game game;
    [SerializeField] private Script_EntryManager entryManager;
    
    public void SaveEntries(Model_SaveData data)
    {
        Model_Entry[] entriesData = new Model_Entry[game.entries.Length];
        
        // get entries from game, create a model for each
        for (var i = 0; i < entriesData.Length; i++)
        {
            Script_Entry e = game.entries[i];
            entriesData[i] = new Model_Entry(
                e.nameId,
                e.text,
                e.headline,
                e.timestamp
            );
        }

        data.entriesData = entriesData;
    }

    public void LoadEntries(Model_SaveData data)
    {
        if (data.entriesData == null)
        {
            if (Debug.isDebugBuild) Debug.Log("No entries data to load.");
            return;
        }

        Model_Entry[] entriesData = data.entriesData;

        foreach (Model_Entry e in entriesData)
        {
            entryManager.AddEntry(
                e.nameId,
                e.text,
                e.timestamp,
                e.headline
            );
        }
    }
}
