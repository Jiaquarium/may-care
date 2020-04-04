using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Script_SaveSystem
{
    // TODO: REMOVE DEV PATH
    // string path = Application.persistentDataPath + "/player.may";
    static string path = "/Users/jamesgu/Desktop";  
    
    public static void SavePlayer(
        Model_PlayerState playerState
    )
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);


        formatter.Serialize(stream, playerState);
        stream.Close();
    }

    public static Model_PlayerState LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Model_PlayerState playerData = formatter.Deserialize(stream) as Model_PlayerState;

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
