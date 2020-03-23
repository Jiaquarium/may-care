using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Script_SaveSystem
{
    // TODO: REMOVE DEV PATH
    // string path = Application.persistentDataPath + "/player.may";
    static string path = "/Users/jamesgu/Desktop";  
    
    public static void SavePlayer(Script_Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        Script_PlayerData playerData = new Script_PlayerData(player);

        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public static Script_PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Script_PlayerData playerData = formatter.Deserialize(stream) as Script_PlayerData;

            stream.Close();

            return playerData;
        }
        else
        {
            Debug.LogError("Player data not found in " + path);
            return null;
        }
    }
}
