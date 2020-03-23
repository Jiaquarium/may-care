using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Script_PlayerData
{
    public string name;

    public Script_PlayerData(Script_Player player)
    {
        name = player.name;
    }
}
