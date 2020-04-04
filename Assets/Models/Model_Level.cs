using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Model_Level
{
    public Model_Player playerData;
    public Model_NPC[] NPCsData;
    public Model_InteractableObject[] InteractableObjectsData;
    public Model_Demon[] DemonsData;
    public GameObject grid;
    public Tilemap tileMap;
    public Tilemap tileMapNull;
    public Tilemap exitsTileMap;
    public int bgMusicAudioClipIndex;
    public string initialState;
    public bool shouldPersistBgThemes;
}
