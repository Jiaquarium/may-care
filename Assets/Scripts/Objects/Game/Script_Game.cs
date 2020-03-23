using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_Game : MonoBehaviour
{
    public Script_Levels Levels;
    public string state;
    public Script_PlayerData playerState;
    public int targetFrameRate;
    public int level = 0;
    private bool exitsDisabled;

    private GameObject grid;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    public Script_Player PlayerPrefab;
    private Script_Player player;
    private List<Script_StaticNPC> NPCs = new List<Script_StaticNPC>();
    public Script_StaticNPC StaticNPCPrefab;
    private List<Script_MovingNPC> movingNPCs = new List<Script_MovingNPC>();
    public Script_MovingNPC MovingNPCPrefab;
    private List<Script_InteractableObject> interactableObjects = new List<Script_InteractableObject>();
    public Script_InteractableObject InteractableObjectPrefab;
    public Script_DialogueManager dialogueManager;
    public Script_BackgroundMusicManager bgMusicManager;
    private AudioSource backgroundMusicAudioSource;
    public Script_InteractableObjectHandler interactableObjectHandler;
    
    
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        Cursor.visible = false;

        backgroundMusicAudioSource = bgMusicManager.GetComponent<AudioSource>();
        dialogueManager.HideDialogue();
        
        InitiateLevel(level);

        // TODO: Initialize State Func
        playerState = new Script_PlayerData(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeStateCutScene()
    {
        state = "cut-scene";
    }

    public void ChangeStateCutSceneNPCMoving()
    {
        state = "cut-scene_npc-moving";
    }

    public void ChangeStateMove()
    {
        state = "move";
    }

    public void ChangeStateInteract()
    {
        state = "interact";
        CameraTargetToPlayer();
    }

    void SetInitialGameState(int i)
    {
        print("setting initial game state: " + Levels.levelsData[i].initialState);
        state = Levels.levelsData[i].initialState;
    }

    public void HandleLevelExit()
    {
        if (exitsDisabled)  return;

        DestroyLevel(level);
        
        level++;
        InitiateLevel(level);
    }

    public void DisableExits()
    {
        exitsDisabled = true;
    }

    public void EnableExits()
    {
        exitsDisabled = false;
    }

    void InitiateLevel(int level)
    {
        SetInitialGameState(level);

        StartBgMusic();
        CreateTileMaps(level);
        CreatePlayer(level);
        CreateNPCs(level);
        SetupDialogueManager();
    }

    void DestroyLevel(int level)
    {
        DestroyPlayer();
        DestroyNPCs();
        DestroyTileMaps();
    }

    void CreateTileMaps(int level)
    {
        grid = Levels.levelsData[level].grid;
        tileMap = Levels.levelsData[level].tileMap;
        exitsTileMap = Levels.levelsData[level].exitsTileMap;

        grid.SetActive(true);
    }

    void DestroyTileMaps()
    {
        grid.SetActive(false);
    }

    void CreatePlayer(int level)
    {
        Script_PlayerModel playerData = Levels.levelsData[level].playerData;
        
        Vector3 spawnLocation = playerData.playerSpawnLocation;
        player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
        player.Setup(tileMap, exitsTileMap, playerData.direction);

        // camera tracking
        Camera.main.GetComponent<Script_Camera>().target = player.transform;
    }

    void DestroyPlayer()
    {
        Destroy(player.gameObject);
    }

    public void SetPlayerState(Dictionary<string, string> state)
    {
        playerState.name = state["name"];
    }

    public Script_PlayerData GetPlayerState()
    {
        return playerState;
    }

    public bool HandleActionToNPC(Vector3 desiredLocation, string action)
    {
        for (int i = 0; i < NPCs.Count; i++)
        {
            // check if it's NPC occupying the spot
            if (desiredLocation.x == NPCs[i].transform.position.x)
            {
                if (action == "Action1" && !player.GetIsTalking())
                {
                    NPCs[i].TriggerDialogue();
                }
                else if (action == "Action1" && player.GetIsTalking())
                {
                    NPCs[i].ContinueDialogue();
                }
                else if (action == "Submit" && player.GetIsTalking())
                {
                    NPCs[i].SkipTypingSentence();
                }

                return true;
            }
        }

        return false;
    }

    public bool HandleActionToInteractableObject(
        Vector3 _desiredLocation,
        string action
    )
    {
        return interactableObjectHandler.HandleAction(
            interactableObjects,
            desiredLocation,
            action
        );
    }

    // for cut scenes, monologues
    public void HandleContinuingDialogueActions(string action)
    {
        if (action == "Action1" && player.GetIsTalking())
        {
            dialogueManager.DisplayNextSentence();
        }
        else if (action == "Submit" && player.GetIsTalking())
        {
            dialogueManager.SkipTypingSentence();
        }
    }

    void CreateNPCs(int level)
    {
        Script_NPCModel[] NPCsData = Levels.levelsData[level].NPCsData;
        
        if (NPCsData.Length == 0)   return;

        for (int i = 0; i < NPCsData.Length; i++)
        {
            if (NPCsData[i].isMovingNPC)
            {
                Script_MovingNPC MovingNPC = Instantiate(
                    MovingNPCPrefab,
                    NPCsData[i].NPCSpawnLocation,
                    Quaternion.identity
                );

                NPCs.Add(MovingNPC);
                movingNPCs.Add(MovingNPC);

                MovingNPC.StaticNPCId = i;
                MovingNPC.MovingNPCId = movingNPCs.Count - 1;
                MovingNPC.Setup(
                    NPCsData[i].sprite,
                    NPCsData[i].dialogue,
                    NPCsData[i].moveSets
                );
            }
            else
            {
                Script_StaticNPC NPC = Instantiate(
                    StaticNPCPrefab,
                    NPCsData[i].NPCSpawnLocation,
                    Quaternion.identity
                );

                NPCs.Add(NPC);
                
                NPC.StaticNPCId = i;
                NPC.Setup(
                    NPCsData[i].sprite,
                    NPCsData[i].dialogue,
                    new Script_MoveSetModel[0] // unneeded for base
                );
            }

        }
    }

    void DestroyNPCs()
    {
        foreach(Script_StaticNPC NPC in NPCs)
        {
            if (NPC)    Destroy(NPC.gameObject);
        }

        // destroy the spaces that are now empty references
        NPCs.Clear();
        movingNPCs.Clear();
    }

    public void DestroyMovingNPC(int i)
    {
        Destroy(movingNPCs[i].gameObject);
        movingNPCs.RemoveAt(i);
    }

    public Vector3[] GetMovingNPCLocations()
    {
        Vector3[] MovingNPCLocations = new Vector3[movingNPCs.Count];
        print("MovingNPCsLength: " + movingNPCs.Count);
        print("MovingNPCLocations: " + MovingNPCLocations);
        
        if (MovingNPCLocations.Length == 0)    return new Vector3[0];

        for (int i = 0; i < movingNPCs.Count; i++)
        {
            MovingNPCLocations[i] = movingNPCs[i].transform.position;
        }

        return MovingNPCLocations;
    }

    public void TriggerMovingNPCMove(int i)
    {
        movingNPCs[i].Move();
    }

    void SetupDialogueManager()
    {
        dialogueManager.Setup();
    }

    void StartBgMusic()
    {
        int i = Levels.levelsData[level].bgMusicAudioClipIndex;
        
        bgMusicManager.Play(i);
    }

    public void PauseBgMusic()
    {
        bgMusicManager.Pause();
    }

    public void UnPauseBgMusic()
    {
        bgMusicManager.UnPause();
    }

    public Vector3 GetPlayerLocation()
    {
        return player.GetComponent<Transform>().position;
    }

    public bool GetPlayerIsTalking()
    {
        return player.GetIsTalking();
    }

    public void PlayerFaceDirection(string direction)
    {
        player.FaceDirection(direction);
    }

    public void ChangeCameraTargetToNPC(int i)
    {
        Camera.main.GetComponent<Script_Camera>().target = NPCs[i].transform;
    }

    public void CameraTargetToPlayer()
    {
        Camera.main.GetComponent<Script_Camera>().target = player.transform;
    }

    public void StartDialogue(Script_Dialogue dialogue)
    {
        dialogueManager.StartDialogue(dialogue);
    }
}
