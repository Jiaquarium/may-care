using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Script_LevelsModel
{
    public Script_PlayerModel playerData;
    public Script_NPCModel[] NPCsData;
    public Script_InteractableObjectsModel[] InteractableObjectsData;
    public Script_DemonModel[] DemonsData;
    public GameObject grid;
    public Tilemap tileMap;
    public Tilemap tileMapNull;
    public Tilemap exitsTileMap;
    public int bgMusicAudioClipIndex;
    public string initialState;
}
