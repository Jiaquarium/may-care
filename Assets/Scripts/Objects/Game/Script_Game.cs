using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Script_Game : MonoBehaviour
{
    /* =======================================================================
        RUNTIME OPTIONS
    ======================================================================= */
    public int targetFrameRate;
    
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public Model_Levels Levels;
    public string state;
    public Model_PlayerState playerState;
    public int level = 0;
    public Model_PlayerThoughts thoughts;
    
    /* ======================================================================= */


    public Script_InteractableObjectHandler interactableObjectHandler;
    public Script_InteractableObjectCreator interactableObjectCreator;
    public Script_DemonHandler demonHandler;
    public Script_DemonCreator demonCreator;
    public Script_CutSceneNPCCreator cutSceneNPCCreator;
    public Script_PlayerThoughtHandler playerThoughtHandler;
    public Script_PlayerThoughtsInventoryManager playerThoughtsInventoryManager;
    public Script_Exits exitsHandler;
    public Script_Player PlayerPrefab;
    public Script_StaticNPC StaticNPCPrefab;
    public Script_MovingNPC MovingNPCPrefab;
    public Script_AudioOneShotSource AudioOneShotSourcePrefab;
    public Script_BgThemePlayer EroBgThemePlayerPrefab;
    public Script_DialogueManager dialogueManager;
    public Script_ThoughtManager thoughtManager;
    public Script_BackgroundMusicManager bgMusicManager;
    public Script_PlayerThoughtsInventoryButton[] thoughtButtons;
    public Font[] fonts;


    private GameObject grid;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    private Tilemap entrancesTileMap;
    private Script_Player player;
    private List<Script_StaticNPC> NPCs = new List<Script_StaticNPC>();
    private List<Script_MovingNPC> movingNPCs = new List<Script_MovingNPC>();
    private List<Script_CutSceneNPC> cutSceneNPCs = new List<Script_CutSceneNPC>();
    private List<Script_InteractableObject> interactableObjects = new List<Script_InteractableObject>();
    private List<Script_Switch> switches = new List<Script_Switch>();
    private List<Script_Demon> demons = new List<Script_Demon>();
    private Script_Demon DemonPrefab;
    private AudioSource backgroundMusicAudioSource;
    private Script_Camera camera;
    private List<Script_AudioOneShotSource> audioOneShotSources = new List<Script_AudioOneShotSource>();
    private Script_BgThemePlayer eroBgThemePlayer;
    private Script_LevelBehavior levelBehavior;
    
    
    private bool isInventoryOpen = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Script_Utils.MakeFontsCrispy(fonts);
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        
        // TODO: UNCOMMENT THIS
        PlayerPrefs.DeleteAll();
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        /*
            set up handlers that affect state
        */
        ChangeStateToInitiateLevel();
        exitsHandler.Setup(this);

        camera = Camera.main.GetComponent<Script_Camera>();
        backgroundMusicAudioSource = bgMusicManager.GetComponent<AudioSource>();
        dialogueManager.HideDialogue();
        thoughtManager.HideThought();
        ClosePlayerThoughtsInventory();
        
        // ChangeStateToInitiateLevel();
        
        if (level == 0)    InitiateLevel();
        else
        {
            // TODO: remove this if stmt, will handle this by updating player state
            Script_TileMapExitEntrance lastLevelExitData = Levels
                .levelsData[level - 1]
                .exitsTileMap.GetComponent<Script_TileMapExitEntrance>();
            int x = (int)lastLevelExitData.playerNextSpawnPosition.x;
            int z = (int)lastLevelExitData.playerNextSpawnPosition.z;
            string dir = lastLevelExitData.playerFacingDirection;
            SetPlayerState(new Model_PlayerState(null, x, z, dir));
            InitiateLevel();
        }

        exitsHandler.canvas.alpha = 1.0f;
        exitsHandler.StartFadeIn();

        // TODO: Initialize State Func
        // playerState = new Model_PlayerState(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeStateToInitiateLevel()
    {
        state = "initiate-level";
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
        CameraMoveToTarget();
    }

    public void SetInitialGameState()
    {
        if (levelBehavior != null)    levelBehavior.InitGameState();
        else ChangeStateInteract();
    }

    public void InitiateLevel()
    {
        StartBgMusic();
        
        CreateTileMaps();
        CreatePlayer();
        CameraMoveToTarget();

        SetupDialogueManager();
        SetupThoughtManager();

        // must occur last to have references set
        InitLevelBehavior();
    }

    void InitLevelBehavior()
    {
        levelBehavior = Levels.levelsData[level].behavior;
        if (levelBehavior == null)  return;
        levelBehavior.Setup();
    }

    public void DestroyLevel()
    {
        DestroyPlayer();
        DestroyNPCs();
        DestroyDemons();
        DestroyAudioOneShotSources();
        DestroyInteractableObjects();
        DestroyTileMaps();
        
        StopMovingNPCThemes();
    }

    void CreateTileMaps()
    {
        grid = Levels.levelsData[level].grid;
        tileMap = Levels.levelsData[level].tileMap;
        exitsTileMap = Levels.levelsData[level].exitsTileMap;
        entrancesTileMap = Levels.levelsData[level].entrancesTileMap;

        grid.SetActive(true);
    }

    void DestroyTileMaps()
    {
        grid.SetActive(false);
    }

    public void SetNewTileMapGround(Tilemap _tileMap)
    {   
        tileMap = _tileMap;   
    }

    public Tilemap GetTileMap()
    {
        return tileMap;
    }

    public Tilemap GetEntrancesTileMap()
    {
        return entrancesTileMap;
    }

    public Tilemap GetExitsTileMap()
    {
        return exitsTileMap;
    }

    void CreatePlayer()
    {
        // TODO don't need this, put all player data into PlayerState 
        Model_Level levelData = Levels.levelsData[level];
        Model_Player playerData = levelData.playerData;

        Vector3 spawnLocation = new Vector3(
            playerState.spawnX ?? 0f,
            0f,
            playerState.spawnZ ?? 0f
        );
        player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
        player.Setup(
            playerState.faceDirection,
            playerState,
            playerData.isLightOn,
            playerData.isReflectionOn,
            playerData.reflectionVector
        );

        // camera tracking
        camera.target = player.transform;
    }

    void DestroyPlayer()
    {
        Destroy(player.gameObject);
    }

    public void SetPlayerState(Model_PlayerState state)
    {
        playerState.name = state.name ?? playerState.name;
        playerState.spawnX = state.spawnX ?? playerState.spawnX;
        playerState.spawnZ = state.spawnZ ?? playerState.spawnZ;
        playerState.faceDirection = state.faceDirection ?? playerState.faceDirection;

        print("spawnX: " + playerState.spawnX + ", spawnZ: " + playerState.spawnZ);
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
        print("action: " + action);
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
            player,
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
            dialogueManager.DisplayNextDialoguePortion();
        }
        else if (action == "Submit" && player.GetIsTalking())
        {
            dialogueManager.SkipTypingSentence();
        }
    }

    public void RemovePlayerReflection()
    {
        player.RemoveReflection();
    }

    public void CreateNPCs()
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
            else if (NPCsData[i].isCutSceneNPC)
            {
                cutSceneNPCCreator.CreateCutSceneNPC(
                    NPCsData[i],
                    NPCs,
                    cutSceneNPCs,
                    i
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
                    new Model_MoveSet[0] // unneeded for base
                );
            }

        }
    }

    public void CreateMovingNPC(
        int i,
        string direction,
        int moveSetIndex = 0,
        bool isActivated = false
    )
    {
        Model_NPC NPCData = Levels.levelsData[level].NPCsData[i];
        Vector3 NPCSpawnLocation = NPCData.NPCSpawnLocation;
        Model_MoveSet[] allMoveSets = NPCData.moveSets;
        Model_MoveSet[] truncatedMoveSet = new Model_MoveSet[
            Mathf.Max(allMoveSets.Length - moveSetIndex - 1, 0)
        ];
        List<Model_MoveSet> movedMoveSets = new List<Model_MoveSet>();

        // NPC has moved its first moveSet
        if (isActivated)
        {
            for (int j = 0, k = moveSetIndex + 1; j < truncatedMoveSet.Length; j++, k++)
            {
                truncatedMoveSet[j] = allMoveSets[k];
            }

            // add moveSets we want to skip over
            for (int j = 0; j < moveSetIndex + 1; j++)  movedMoveSets.Add(allMoveSets[j]);

            foreach(Model_MoveSet ms in movedMoveSets)
            {
                print(ms);
                NPCSpawnLocation += Script_Utils.MovesToVector(ms);
            }
        }

        Script_MovingNPC MNPC = Instantiate(
            MovingNPCPrefab,
            NPCSpawnLocation,
            Quaternion.identity
        );

        NPCs.Add(MNPC);
        movingNPCs.Add(MNPC);

        MNPC.StaticNPCId = NPCs.Count - 1;
        MNPC.MovingNPCId = movingNPCs.Count - 1;
        MNPC.Setup(
            NPCData.sprite,
            NPCData.dialogue,
            isActivated ? truncatedMoveSet : allMoveSets
        );
        MNPC.FaceDirection(direction ?? NPCData.direction);
    }

    void DestroyNPCs()
    {
        foreach(Script_StaticNPC NPC in NPCs)
        {
            if (NPC)    Destroy(NPC.gameObject);
        }

        NPCs.Clear();
        movingNPCs.Clear();
        cutSceneNPCs.Clear();
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

    public void CurrentMovesDoneAction()
    {
        // if (!bgMusicManager.GetIsPlaying())    UnPauseBgMusic();
        
        // if (eroBgThemePlayer != null)
        // {
        //     PauseEroTheme();
        // }
    }

    public void AllMovesDoneAction(int i)
    {
        // if ero is not exiting, then continue his theme
        if (movingNPCs[i].shouldExit)
        {
            StopMovingNPCThemes();
            UnPauseBgMusic();
        }
        // otherwise, continue playing movingNPC theme music until player exits
    }

    public void CreateInteractableObjects(bool[] switchesState)
    {
        interactableObjectCreator.CreateInteractableObjects(
            Levels.levelsData[level].InteractableObjectsData,
            interactableObjects,
            switches,
            GetRotationToFaceCamera(),
            dialogueManager,
            player,
            switchesState
        );
    }

    void DestroyInteractableObjects()
    {
        interactableObjectCreator.DestroyInteractableObjects(
            interactableObjects, 
            switches
        );    
    }

    public Vector3[] GetInteractableObjectLocations()
    {
        return interactableObjectHandler.GetLocations(interactableObjects);
    }

    public List<Script_InteractableObject> GetInteractableObjects()
    {
        return interactableObjects;
    }

    public int GetSwitchesCount()
    {
        return switches.Count;
    }

    public void SetSwitchesState(bool[] switchesState)
    {
        for (int i = 0; i < switches.Count; i++)
        {
            switches[i].isOn = switchesState[i];
        }
    }

    public void SetSwitchState(int Id, bool isOn)
    {
        levelBehavior.SetSwitchState(Id, isOn);
    }

    public Script_Switch GetSwitch(int Id)
    {
        return switches[Id];
    }

    public void CreateDemons(bool[] spawnState)
    {
        demonCreator.CreateDemons(
            spawnState,
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
        levelBehavior.EatDemon(Id);
    }

    public void PlayerEatDemonHeart()
    {
        if (player.GetIsEating())
        {
            player.EatHeart();
        }
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

    public Script_AudioOneShotSource CreateAudioOneShotSource(Vector3 position)
    {
        Script_AudioOneShotSource a = Instantiate(
            AudioOneShotSourcePrefab,
            position,
            Quaternion.identity
        );

        audioOneShotSources.Add(a);

        return a;
    }

    void DestroyAudioOneShotSources()
    {
        foreach(Script_AudioOneShotSource a in audioOneShotSources)
        {
            if (a)    Destroy(a.gameObject);
        }

        audioOneShotSources.Clear();
    }

    void SetupDialogueManager()
    {
        dialogueManager.Setup();
    }

    public void StartDialogue(Model_Dialogue dialogue)
    {
        dialogueManager.StartDialogue(dialogue, null);
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
        // TODO: make this a general theme player, not just for Ero
        if (eroBgThemePlayer != null && GetNPCThemeMusicIsPlaying())   return;
        
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

    public void PlayEroTheme()
    {
        eroBgThemePlayer = Instantiate(
            EroBgThemePlayerPrefab,
            player.transform.position,
            Quaternion.identity
        );
    }

    public void PauseEroTheme()
    {
        eroBgThemePlayer.GetComponent<AudioSource>().Pause();
    }

    public void UnPauseEroTheme()
    {
        if (eroBgThemePlayer == null)
        {
            Debug.Log("No eroBgThemePlayer object exists to UnPause.");
            return;
        }
        eroBgThemePlayer.GetComponent<AudioSource>().UnPause();
    }

    public void StopBgTheme()
    {
        eroBgThemePlayer.GetComponent<AudioSource>().Stop();
        Destroy(eroBgThemePlayer.gameObject);

        eroBgThemePlayer = null;
    }

    public bool GetEroThemeActive()
    {
        return  eroBgThemePlayer != null;
    }

    public bool GetNPCThemeMusicIsPlaying()
    {
        return eroBgThemePlayer.GetComponent<AudioSource>().isPlaying;
    }

    public void StopMovingNPCThemes()
    {
        // TODO: call action in LevelBehavior that will handle
        print("ero bg theme player is: " + eroBgThemePlayer);

        if (eroBgThemePlayer == null)   return;

        if (Levels.levelsData[level].shouldPersistBgThemes)    PauseEroTheme();
        
        StopBgTheme();
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
        camera.SetTarget(NPCs[i].transform);
        // move camera fast
        CameraMoveToTarget();
    }

    public void ChangeCameraTargetToGameObject(GameObject obj)
    {
        camera.SetTarget(obj.transform);
        CameraMoveToTarget();
    }

    public void CameraTargetToPlayer()
    {
        camera.target = player.transform;
    }

    public void CameraMoveToTarget()
    {
        camera.MoveToTarget();
    }

    public Vector3 GetRotationToFaceCamera()
    {
        return camera.GetRotationAdjustment();
    }

    public void EnableExits()
    {
        exitsHandler.EnableExits();
    }

    public void DisableExits()
    {
        exitsHandler.DisableExits();
    }

    public bool GetIsExitsDisabled()
    {
        return exitsHandler.GetIsExitsDisabled();
    }

    public void Exit(
        int level,
        Vector3 playerNextSpawnPosition,
        string playerFacingDirection,
        bool isExit
    )
    {
        exitsHandler.Exit(
            level,
            playerNextSpawnPosition,
            playerFacingDirection,
            isExit
        );
    }
}
