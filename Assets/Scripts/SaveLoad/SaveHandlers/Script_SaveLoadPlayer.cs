using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SaveLoadPlayer : MonoBehaviour
{
    [SerializeField] private Script_Game game;

    public void SavePlayerData(Model_SaveData data)
    {
        game.UpdatePlayerStateToCurrent();
        Model_PlayerState p = game.GetPlayerState();
        data.playerData = new Model_PlayerState(
            p.name,
            p.spawnX,
            p.spawnY,
            p.spawnZ,
            p.faceDirection
        );
    }

    public void LoadPlayerData(Model_SaveData data)
    {
        Model_PlayerState p = new Model_PlayerState(
            data.playerData.name,
            data.playerData.spawnX,
            data.playerData.spawnY,
            data.playerData.spawnZ,
            data.playerData.faceDirection
        );
        game.SetPlayerState(p);
    }
}