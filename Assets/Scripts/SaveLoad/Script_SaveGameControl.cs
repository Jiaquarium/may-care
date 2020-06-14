using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Script_SaveGameControl : MonoBehaviour
{
    public static Script_SaveGameControl control;
    public static string path;
    private string devPath = "/Users/jamesgu/Desktop/gameInfo.dat";
    
    public Script_SaveLoadGame gameHandler;
    public Script_SaveLoadPlayer playerHandler;
    public Script_SaveLoadEntries entriesHandler;

    void Awake() {
        control = this;
        if (control != this)    Destroy(this.gameObject);

        if (Debug.isDebugBuild && Const_Dev.IsDevMode) path = devPath;
        else                                           path = Application.persistentDataPath + "/gameInfo.dat";
    }

    public virtual void OnInspectorGUI(){}

    public void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            // will overwrite existing file
            FileStream file = File.Create(path);

            Model_SaveData data = new Model_SaveData();
            
            // modify with all necessary persistent state data
            gameHandler.SaveGameData(data);
            playerHandler.SavePlayerData(data);
            entriesHandler.SaveEntries(data);

            bf.Serialize(file, data);
            file.Close();

            if (Debug.isDebugBuild) Debug.Log("Save successful at: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed Save with exception: " + e.ToString());
        }
    }

    public bool Load()
    {
        try 
        {
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                Model_SaveData data = (Model_SaveData)bf.Deserialize(file);
                file.Close();

                gameHandler.LoadGameData(data);
                playerHandler.LoadPlayerData(data);
                entriesHandler.LoadEntries(data);

                if (Debug.isDebugBuild) Debug.Log("Successful load at: " + path);
                return true;
            }
            else
            {
                if (Debug.isDebugBuild) Debug.Log("Could not load; file not found.");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed Load with exception: " + e.ToString());
            return false;
        }
    }

    public void Delete()
    {
        // TODO DELETE
        if (Debug.isDebugBuild && Const_Dev.IsDevMode) path = devPath;
        
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                if (Debug.isDebugBuild) Debug.Log("Save file deleted.");
            }
            else
            {
                if (Debug.isDebugBuild) Debug.Log("Could not delete; file not found.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed Delete with exception: " + e.ToString());
        }
    }
}