using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_SaveData
{
    public Model_GameData gameData { get; set; }
    public Model_PlayerState playerData { get; set; }
    public Model_Entry[] entriesData { get; set; }
}