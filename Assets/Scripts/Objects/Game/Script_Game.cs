using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_Game : MonoBehaviour
{

    
    public Script_InteractableObjectHandler interactableObjectHandler;
    public Script_InteractableObjectCreator interactableObjectCreator;
    public Script_DemonHandler demonHandler;
    public Script_DemonCreator demonCreator;
    public Script_PlayerThoughtHandler playerThoughtHandler;
    public Script_PlayerThoughtsInventoryManager playerThoughtsInventoryManager;
    public Script_Player PlayerPrefab;
    public Script_StaticNPC StaticNPCPrefab;
    public Script_MovingNPC MovingNPCPrefab;
    public Script_DialogueManager dialogueManager;
    public Script_ThoughtManager thoughtManager;
    public Script_BackgroundMusicManager bgMusicManager;
    public Script_PlayerThoughtsInventoryButton[] thoughtButtons;


    private GameObject grid;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    private Script_Player player;
    private List<Script_StaticNPC> NPCs = new List<Script_StaticNPC>();
    private List<Script_MovingNPC> movingNPCs = new List<Script_MovingNPC>();
    private List<Script_InteractableObject> interactableObjects = new List<Script_InteractableObject>();
    private List<Script_Demon> demons = new List<Script_Demon>();
    private Script_Demon DemonPrefab;
    private AudioSource backgroundMusicAudioSource;
    
    
    public Model_Levels Levels;
    public string state;
    public Model_PlayerState playerState;
    public int targetFrameRate;
    public int level = 0;
    public Model_PlayerThoughts thoughts;
    
    
    private bool isInventoryOpen = false;
    private bool exitsDisabled;
    
    
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
        thoughtManager.HideThought();
        ClosePlayerThoughtsInventory();
        
        InitiateLevel();

        // TODO: Initialize State Func
        // playerState = new Model_PlayerState(player);
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

    public void ChangeStateInteract()
    {
        state = "interact";
        CameraTargetToPlayer();
    }

    void SetInitialGameState()
    {
        state = Levels.levelsData[level].initialState;
    }

    public void HandleLevelExit()
    {
        if (exitsDisabled)  return;

        DestroyLevel();
        
        level++;
        InitiateLevel();
    }

    public void DisableExits()
    {
        exitsDisabled = true;
    }

    public void EnableExits()
    {
        exitsDisabled = false;
    }

    void InitiateLevel()
    {
        SetInitialGameState();

        StartBgMusic();
        
        CreateTileMaps();
        CreatePlayer();
        CreateNPCs();
        CreateInteractableObjects();
        CreateDemons();

        SetupDialogueManager();
        SetupThoughtManager();
    }

    void DestroyLevel()
    {
        DestroyPlayer();
        DestroyNPCs();
        DestroyDemons();
        DestroyInteractableObjects();
        DestroyTileMaps();
    }

    void CreateTileMaps()
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

    void CreatePlayer()
    {
        Model_Player playerData = Levels.levelsData[level].playerData;
        
        Vector3 spawnLocation = playerData.playerSpawnLocation;
        player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
        player.Setup(
            tileMap,
            exitsTileMap,
            playerData.direction,
            playerState
        );

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

    public Model_PlayerState GetPlayerState()
    {
        return playerState;
    }

    public void AddPlayerThought(Model_Thought thought)
    {
        playerThoughtHandler.AddPlayerThought(thought, thoughts);
        
        int thoughtCount = playerThoughtHandler.GetThoughtsCount(thoughts);
        
        playerThoughtsInventoryManager.AddPlayerThought(
            thought, thoughtButtons[thoughtCount - 1]
        );
    }

    public bool HandleActionToNPC(Vector3 desiredLocation, string action)
    {
        for (int i = 0; i < NPCs.Count; i++)
        {
            if (NPCs[i] == null)    return false;

            // check if it's NPC occupying the spot
            if (
                desiredLocation.x == NPCs[i].transform.position.x
                && desiredLocation.z == NPCs[i].transform.position.z
            )
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
        Vector3 desiredLocation,
        string action
    )
    {
        return interactableObjectHandler.HandleAction(
            interactableObjects,
            desiredLocation,
            action
        );
    }

    public bool HandleActionToDemon(
        Vector3 desiredLocation,
        string action    
    )
    {
        return demonHandler.HandleAction(
            demons,
            desiredLocation,
            action
        );
    }

    public void OpenPlayerThoughtsInventory()
    {
        bool hasThoughts = playerThoughtHandler.GetThoughtsCount(thoughts) > 0;
        
        isInventoryOpen = true;
        playerThoughtsInventoryManager.OpenInventory(
            hasThoughts
        );
    }

    public void ClosePlayerThoughtsInventory()
    {
        isInventoryOpen = false;
        playerThoughtsInventoryManager.CloseInventory();
    }

    public bool GetIsInventoryOpen()
    {
        return isInventoryOpen;
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

    // TODO: remove this int (dont need, can just use global reference?)
    void CreateNPCs()
    {
        Model_NPC[] NPCsData = Levels.levelsData[level].NPCsData;
        
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
                // setup animator for starting idle position
                MovingNPC.FaceDirection(NPCsData[i].direction);
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
                    new Model_MoveSet[0] // unneeded for base
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

        NPCs.Clear();
        movingNPCs.Clear();
    }

    public void DestroyMovingNPC(int Id)
    {
        for (int i = 0; i < movingNPCs.Count; i++)
        {
            if (movingNPCs[i].StaticNPCId == Id)
            {
                Destroy(movingNPCs[i].gameObject);
                movingNPCs.RemoveAt(i);
            }
        }
    }

    public Vector3[] GetMovingNPCLocations()
    {
        Vector3[] MovingNPCLocations = new Vector3[movingNPCs.Count];
        
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

    public void SetMovingNPCExit(int i, bool shouldExit)
    {
        movingNPCs[i].shouldExit = shouldExit;
    }

    public void ChangeMovingNPCSpeed(int i, float speed)
    {
        movingNPCs[i].ChangeSpeed(speed);
    }

    public void CreateInteractableObjects()
    {
        interactableObjectCreator.CreateInteractableObjects(
            Levels.levelsData[level].InteractableObjectsData,
            interactableObjects
        );
    }

    void DestroyInteractableObjects()
    {
        interactableObjectCreator.DestroyInteractableObjects(interactableObjects);    
    }

    public void CreateDemons()
    {
        demonCreator.CreateDemons(
            Levels.levelsData[level].DemonsData,
            demons
        );
    }

    void DestroyDemons()
    {
        foreach(Script_Demon d in demons)
        {
            if (d)    Destroy(d.gameObject);
        }

        demons.Clear();
    }

    public void EatDemon(int Id)
    {
        demonHandler.EatDemon(Id, demons);
    }

    public Vector3[] GetDemonLocations()
    {
         Vector3[] DemonLocations = new Vector3[demons.Count];
        
        if (DemonLocations.Length == 0)    return new Vector3[0];

        for (int i = 0; i < demons.Count; i++)
        {
            DemonLocations[i] = demons[i].transform.position;
        }

        return DemonLocations;
    }

    public int GetDemonsCount()
    {
        return demons.Count;
    }

    void SetupDialogueManager()
    {
        dialogueManager.Setup();
    }

    public void StartDialogue(Model_Dialogue dialogue)
    {
        dialogueManager.StartDialogue(dialogue);
    }

    void SetupThoughtManager()
    {
        thoughtManager.Setup();
    }

    public void ShowAndCloseThought(Model_Thought thought)
    {
        thoughtManager.ShowThought(thought);
        thoughtManager.CloseThought(thought);
    }

    void StartBgMusic()
    {
        int i = Levels.levelsData[level].bgMusicAudioClipIndex;
        
        bgMusicManager.Play(i);
    }

    public void SwitchBgMusic(int i)
    {
        bgMusicManager.Play(i, forcePlay: true);
    }

    public void StopBgMusic()
    {
        bgMusicManager.Stop();
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

    public void NPCFaceDirection(int Id, string direction)
    {
        foreach(Script_MovingNPC NPC in movingNPCs)
        {
            if (NPC.MovingNPCId == Id)   NPC.FaceDirection(direction);
        }
    }

    public void ChangeCameraTargetToNPC(int i)
    {
        Camera.main.GetComponent<Script_Camera>().target = NPCs[i].transform;
    }

    public void CameraTargetToPlayer()
    {
        Camera.main.GetComponent<Script_Camera>().target = player.transform;
    }    
}
