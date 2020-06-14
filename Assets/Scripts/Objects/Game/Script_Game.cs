using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Script_Game : MonoBehaviour
{
    
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public int level = 0;
    
    /* -----------------------------------------------------------------------
        RUNTIME SYSTEM OPTIONS
    ----------------------------------------------------------------------- */
    public int targetFrameRate;
    /* -------------------------------------------------------------------- */

    public Model_Levels Levels;
    public string state;
    public Model_PlayerState playerState;
    public Model_PlayerThoughts thoughts;
    public Script_PlayerThoughtsInventoryButton[] thoughtSlots;
    public Vector3 levelZeroCameraPosition;
    
    /* ======================================================================= */

    public static Script_Game Game;
    public Script_DDRManager DDRManager;
    public Script_DDRHandler DDRHandler;
    public Script_InteractableObjectHandler interactableObjectHandler;
    public Script_InteractableObjectCreator interactableObjectCreator;
    public Script_DemonHandler demonHandler;
    public Script_DemonCreator demonCreator;
    public Script_MovingNPCCreator movingNPCCreator;
    public Script_CutSceneNPCCreator cutSceneNPCCreator;
    public Script_SavePointCreator savePointCreator;
    public Script_ReflectionCreator reflectionCreator;
    public Script_PlayerThoughtHandler playerThoughtHandler;
    public Script_PlayerThoughtsInventoryManager playerThoughtsInventoryManager;
    public Script_HealthManager healthManager;
    public Script_TransitionManager transitionManager;
    public Script_SceneManager sceneManager;
    public Script_CutSceneManager cutSceneManager;
    public Script_Exits exitsHandler;
    public Script_DialogueManager dialogueManager;
    public Script_ThoughtManager thoughtManager;
    public Script_HintManager hintManager;
    public Script_BackgroundMusicManager bgMusicManager;
    public Script_EntryManager entryManager;

    public Script_Player PlayerPrefab;
    public Script_StaticNPC StaticNPCPrefab;
    public Script_MovingNPC MovingNPCPrefab;
    public Script_MovingNPC MovingNPCIdsPrefab;
    public Script_AudioOneShotSource AudioOneShotSourcePrefab;
    public Script_BgThemePlayer EroBgThemePlayerPrefab;

    public Font[] fonts;
    public Script_Camera camera;
    public GameObject canvases;
    public Transform world;
    public Transform playerContainer;
    public Transform bgThemeSpeakersContainer;
    public Transform tmpTargetsContainer;


    public Script_Entry[] entries; 
    public GameObject grid;
    private Tilemap tileMap;
    private Tilemap[] exitsTileMaps;
    private Tilemap entrancesTileMap;
    private Tilemap pushableTileMap;


    private Script_Player player;
    [SerializeField] private Script_SavePoint savePoint; // max 1 per Level
    private List<Script_StaticNPC> NPCs = new List<Script_StaticNPC>();
    private List<Script_MovingNPC> movingNPCs = new List<Script_MovingNPC>();
    private List<Script_CutSceneNPC> cutSceneNPCs = new List<Script_CutSceneNPC>();
    private List<Script_InteractableObject> interactableObjects = new List<Script_InteractableObject>();
    private List<Script_Switch> switches = new List<Script_Switch>();
    private List<Script_Pushable> pushables = new List<Script_Pushable>();
    private List<Script_Demon> demons = new List<Script_Demon>();
    private Script_Demon DemonPrefab;
    private AudioSource backgroundMusicAudioSource;
    private List<Script_AudioOneShotSource> audioOneShotSources = new List<Script_AudioOneShotSource>();
    private Script_BgThemePlayer npcBgThemePlayer;
    private Script_LevelBehavior levelBehavior;
    private Vector3 worldOffset;
    private string lastState;
    [SerializeField]
    private bool isLoadedGame;
    

    void Awake()
    {
        Game = this;
        if (Game != this)   Destroy(gameObject);   
    }

    // Start is called before the first frame update
    void Start()
    {
        Script_Utils.MakeFontsCrispy(fonts);
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        
        PlayerPrefs.DeleteAll();
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        /*
            set up handlers that affect state
        */
        ChangeStateToInitiateLevel();
        exitsHandler.Setup(this);

        canvases.SetActive(true);

        worldOffset = world.position;
        camera = Camera.main.GetComponent<Script_Camera>();
        camera.Setup(levelZeroCameraPosition);
        backgroundMusicAudioSource = bgMusicManager.GetComponent<AudioSource>();
        dialogueManager.HideDialogue();
        thoughtManager.HideThought();
        playerThoughtsInventoryManager.Setup();
        healthManager.Setup();
        cutSceneManager.Setup();
        DDRManager.Deactivate();
        
        if (!Debug.isDebugBuild || Const_Dev.IsPersisting)
        {
            isLoadedGame = Script_SaveGameControl.control.Load();
        }
        
        // TODO: REMOVE (only for dev)
        if (!isLoadedGame && Debug.isDebugBuild && level != 0)
        {
            // TODO: remove this if stmt, will handle this by updating player state
            Script_TileMapExitEntrance lastLevelExitData = Levels
                .levelsData[level - 1]
                .exitsTileMaps[0].GetComponent<Script_TileMapExitEntrance>();
            int x = (int)lastLevelExitData.playerNextSpawnPosition.x;
            int y = (int)lastLevelExitData.playerNextSpawnPosition.y;
            int z = (int)lastLevelExitData.playerNextSpawnPosition.z;
            string dir = lastLevelExitData.playerFacingDirection;
            
            SetPlayerState(new Model_PlayerState(null, x, y, z, dir));
            CreatePlayer();

            InitiateLevel();
        }
        else
        {
            // player creation must happen before level creation as LB needs reference to player
            // at times
            CreatePlayer();
            InitiateLevel();
        }

        exitsHandler.canvas.alpha = 1.0f;
        exitsHandler.StartFadeIn();

        // TODO: Initialize State Func
        // playerState = new Model_PlayerState(player);
    }
    
    public void ChangeStateToInitiateLevel()
    {
        lastState = state;
        state = Const_States_Game.InitiateLevel;
    }

    public void ChangeStateCutScene()
    {
        lastState = state;
        state = Const_States_Game.CutScene;
    }

    public void ChangeStateCutSceneNPCMoving()
    {
        lastState = state;
        state = Const_States_Game.CutSceneNPCMoving;
    }

    public void ChangeStateInteract()
    {
        lastState = state;
        state = Const_States_Game.Interact;
        CameraTargetToPlayer();
    }

    public void ChangeStateToInventory()
    {
        lastState = state;
        state = Const_States_Game.Inventory;
    }

    public void ChangeStateDDR()
    {
        lastState = state;
        state = Const_States_Game.DDR;
    }

    public void ChangeStateToConvo()
    {
        lastState = state;
        state = Const_States_Game.Convo;
    }

    public void InitiateLevel()
    {
        StartBgMusic();
        
        CreateTileMaps();
        // CreatePlayer();
        SetupPlayerOnLevel();
        CameraMoveToTarget();

        SetupDialogueManager();
        SetupThoughtManager();
        SetupHintManager();

        // must occur last to have references set
        InitLevelBehavior();
    }

    void InitLevelBehavior()
    {
        levelBehavior = Levels.levelsData[level].behavior;
        if (levelBehavior == null)  return;
        print($"level: {level}; levelBehavior: {levelBehavior}");
        levelBehavior.Setup();
    }

    public void DestroyLevel()
    {
        levelBehavior.Cleanup();
        
        ClearNPCs();
        ClearInteractableObjects();
        ClearSavePoint();
        ClearDemons();
        DestroyTmpTargets();
        DestroyAudioOneShotSources();
        ClearTilemaps();
        
        StopMovingNPCThemes();
    }

    /* =======================================================================
        _LEVEL BEHAVIOR_
    ======================================================================= */
    public void ActivateTrigger(string Id)
    {
        levelBehavior.ActivateTrigger(Id);
    }

    public void ActivateObjectTrigger(string Id, Collider col)
    {
        levelBehavior.ActivateObjectTrigger(Id, col);
    }

    /* =======================================================================
        _LEVEL MANAGEMENT_
    ======================================================================= */
    // remove temporary targets (used for camera targeting)
    public void DestroyTmpTargets()
    {
        foreach(Transform t in tmpTargetsContainer)
        {
            Destroy(t.gameObject);
        }
    }

    /* =======================================================================
        _TILEMAPS_
    ======================================================================= */

    void CreateTileMaps()
    {
        grid = Levels.levelsData[level].grid;
        tileMap = Levels.levelsData[level].tileMap;
        exitsTileMaps = Levels.levelsData[level].exitsTileMaps;
        entrancesTileMap = Levels.levelsData[level].entrancesTileMap;
        pushableTileMap = Levels.levelsData[level].pushableTileMap;

        grid.SetActive(true);
    }

    void ClearTilemaps()
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

    public Tilemap[] GetExitsTileMaps()
    {
        return exitsTileMaps;
    }

    public Tilemap GetPushablesTileMap()
    {
        return pushableTileMap;
    }

    /* =======================================================================
        _PLAYER_
    ======================================================================= */

    void CreatePlayer()
    {
        // TODO don't need this, put all player data into PlayerState 
        Model_Level levelData = Levels.levelsData[level];
        Model_Player playerData = levelData.playerData;

        Vector3 spawnLocation = new Vector3(
            playerState.spawnX ?? 0f,
            playerState.spawnY ?? 0f,
            playerState.spawnZ ?? 0f
        );
        player = Instantiate(PlayerPrefab, spawnLocation, Quaternion.identity);
        player.Setup(
            playerState.faceDirection,
            playerState,
            playerData.isLightOn
        );
        player.transform.SetParent(playerContainer, false);
        // camera tracking
        camera.target = player.transform;
    }

    public void SetupPlayerOnLevel()
    {
        Model_Level levelData = Levels.levelsData[level];
        Model_Player playerData = levelData.playerData;   

        player.InitializeOnLevel(
            playerState,
            playerData.isLightOn,
            grid.transform
        );

        PlayerForceSortingLayer(
            playerData.isForceSortingLayer,
            playerData.isForceSortingLayerAxisZ
        );
    }

    public void DestroyPlayer()
    {
        Destroy(player.gameObject);
    }

    public void SetPlayerState(Model_PlayerState state)
    {
        playerState.name = state.name ?? playerState.name;
        Script_Names.Player = playerState.name;
        playerState.spawnX = state.spawnX ?? playerState.spawnX;
        playerState.spawnY = state.spawnY ?? playerState.spawnY;
        playerState.spawnZ = state.spawnZ ?? playerState.spawnZ;
        playerState.faceDirection = state.faceDirection ?? playerState.faceDirection;
    }

    public void UpdatePlayerStateToCurrent()
    {
        Model_PlayerState p = new Model_PlayerState(
            playerState.name,
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(player.transform.position.z),
            player.facingDirection
        );
        SetPlayerState(p);
    }

    public Model_PlayerState GetPlayerState()
    {
        return playerState;
    }

    public Script_Player GetPlayer()
    {
        return player;
    }

    public Script_PlayerGhost GetPlayerGhost()
    {
        return player.GetPlayerGhost();
    }

    public Script_PlayerMovementAnimator GetPlayerMovementAnimator()
    {
        return player.GetPlayerMovementAnimator();
    }

    public void AddPlayerThought(Model_Thought thought)
    {
        playerThoughtHandler.AddPlayerThought(thought, thoughts);
        
        int thoughtCount = playerThoughtHandler.GetThoughtsCount(thoughts);
        
        playerThoughtsInventoryManager.AddPlayerThought(
            thought, thoughtSlots[thoughtCount - 1]
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
                if (action == Const_KeyCodes.Action1 && !player.GetIsTalking())
                {
                    NPCs[i].TriggerDialogue();
                }
                else if (action == Const_KeyCodes.Action1 && player.GetIsTalking())
                {
                    NPCs[i].ContinueDialogue();
                }
                else if (action == Const_KeyCodes.Skip && player.GetIsTalking())
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

    public int GetThoughtsCount()
    {
        return playerThoughtHandler.GetThoughtsCount(thoughts);
    }

    // for cut scenes, monologues
    public void HandleContinuingDialogueActions(string action)
    {
        if (action == Const_KeyCodes.Action1 && player.GetIsTalking())
        {
            dialogueManager.DisplayNextDialoguePortion();
        }
        else if (action == Const_KeyCodes.Skip && player.GetIsTalking())
        {
            dialogueManager.SkipTypingSentence();
        }
    }

    public void CreatePlayerReflection(Vector3 axis)
    {
        player.CreatePlayerReflection(axis);
    }
    
    public void RemovePlayerReflection()
    {
        player.RemoveReflection();
    }

    public void PlayerForceSortingLayer(bool isForceSortingLayer, bool isAxisZ)
    {
        player.ForceSortingLayer(isForceSortingLayer, isAxisZ);
    }

    public Vector3 GetPlayerLocation()
    {
        return player.GetComponent<Transform>().position;
    }

    public Transform GetPlayerTransform()
    {
        return player.GetComponent<Transform>();
    }

    public bool GetPlayerIsTalking()
    {
        return player.GetIsTalking();
    }

    public void PlayerFaceDirection(string direction)
    {
        player.FaceDirection(direction);
    }

    public bool GetPlayerIsSpawned()
    {
        return player != null;
    }

    public void PlayerEffectQuestion(bool isShow)
    {
        player.QuestionMark(isShow);
    }

    /* =======================================================================
        _REFLECTION_
    ======================================================================= */
    public void SetupPlayerReflection(Transform r)
    {
        reflectionCreator.SetupPlayerReflection(r);
    }


    /* =======================================================================
        _INVENTORY_
    ======================================================================= */
    public void OpenInventory()
    {
        lastState = state;
        ChangeStateToInventory();
        playerThoughtsInventoryManager.OpenInventory();
    }

    public void CloseInventory()
    {
        state = lastState;
        lastState = Const_States_Game.Inventory;
        player.SetLastState();
        playerThoughtsInventoryManager.CloseInventory();
        levelBehavior.OnCloseInventory();
    }

    public void EnableSBook(bool isActive)
    {
        playerThoughtsInventoryManager.EnableSBook(isActive);
    }

    public void UpdateEntries(Script_Entry[] e)
    {
        entries = e;
    }

    public void ClearEntries()
    {
        foreach (Script_Entry e in entries)
        {
            if (!Application.isEditor) Destroy(e.gameObject);
        }
        entries = new Script_Entry[0];
    }

    /* =======================================================================
        _HEALTH_
    ======================================================================= */
    public void FillHearts(int i)
    {
        healthManager.FillHearts(i);
    }

    public void HideHealth()
    {
        healthManager.Close();
    }

    public void ShowHealth()
    {
        healthManager.Open();
    }

    /* =======================================================================
        _CANVASES_
    ======================================================================= */
    public void ShowHint(string s)
    {
        hintManager.ShowHint(s);
    }

    public void HideHint()
    {
        hintManager.HideHint();
    }

    public void SetupHintManager()
    {
        hintManager.Setup();
    }

    // separate vs. default level fader
    public IEnumerator TransitionFadeIn(float t, Action action)
    {
        return transitionManager.FadeIn(t, action);
    }

    public void ManagePlayerViews(string type)
    {
        if (type == Const_States_PlayerViews.DDR)
        {
            HideHealth();
        }

        if (type == Const_States_PlayerViews.Health)
        {
            ShowHealth();
        }
    }

    /* =======================================================================
        _NPCs_
    ======================================================================= */

    public void SetupCutSceneNPC(Script_CutSceneNPC cutSceneNPC)
    {
        cutSceneNPCCreator.SetupCutSceneNPC(cutSceneNPC, NPCs, cutSceneNPCs);
    }

    public void SetupMovingNPC(Script_MovingNPC movingNPC, bool isInitialize)
    {
        movingNPCCreator.SetupMovingNPC(movingNPC, NPCs, movingNPCs, isInitialize);
    }

    // TODO: DELETE
    public void CreateNPCs()
    {
        Model_NPC[] NPCsData = Levels.levelsData[level].NPCsData;
        
        if (NPCsData.Length == 0)   return;

        for (int i = 0; i < NPCsData.Length; i++)
        {
            if (NPCsData[i].isMovingNPC)
            {
                Script_MovingNPC prefab;
                
                if (NPCsData[i].type == "ids")
                {
                    prefab = MovingNPCIdsPrefab;
                }
                else
                {
                    prefab = MovingNPCPrefab;
                }
                
                Script_MovingNPC MovingNPC = Instantiate(
                    prefab,
                    NPCsData[i].NPCSpawnLocation,
                    Quaternion.identity
                );

                NPCs.Add(MovingNPC);
                movingNPCs.Add(MovingNPC);

                MovingNPC.StaticNPCId = i;
                MovingNPC.MovingNPCId = movingNPCs.Count - 1;
                MovingNPC.Setup(
                    NPCsData[i].dialogue,
                    NPCsData[i].dialogueNodes,
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
                    NPCsData[i].dialogue,
                    NPCsData[i].dialogueNodes,
                    new Model_MoveSet[0] // unneeded for base
                );
            }

        }
    }

    // TODO: DELETE
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
                NPCSpawnLocation += Script_Utils.MovesToVector(ms);
            }
        }

        Script_MovingNPC prefab;

        if (NPCData.type == "ids")
        {
            prefab = MovingNPCIdsPrefab;
        }
        else
        {
            prefab = MovingNPCPrefab;
        }
        
        Script_MovingNPC MNPC = Instantiate(
            prefab,
            NPCSpawnLocation,
            Quaternion.identity
        );

        NPCs.Add(MNPC);
        movingNPCs.Add(MNPC);

        MNPC.StaticNPCId = NPCs.Count - 1;
        MNPC.MovingNPCId = movingNPCs.Count - 1;
        MNPC.Setup(
            NPCData.dialogue,
            NPCData.dialogueNodes,
            isActivated ? truncatedMoveSet : allMoveSets
        );
        MNPC.FaceDirection(
            movedMoveSets.Count > 0 ?
            movedMoveSets[movedMoveSets.Count - 1].endFaceDirection
            : NPCData.direction
        );
    }

    public void DestroyNPCs()
    {
        foreach(Script_StaticNPC NPC in NPCs)
        {
            if (NPC != null)    Destroy(NPC.gameObject);
        }

        NPCs.Clear();
        movingNPCs.Clear();
        cutSceneNPCs.Clear();
    }

    public void ClearNPCs()
    {
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

    public void DestroyCutSceneNPC(int Id)
    {
        for (int i = 0; i < cutSceneNPCs.Count; i++)
        {
            if (cutSceneNPCs[i].StaticNPCId == Id)
            {
                Destroy(cutSceneNPCs[i].gameObject);
                cutSceneNPCs.RemoveAt(i);
            }
        }
    }

    public Vector3[] GetNPCLocations()
    {
        Vector3[] NPCLocations = new Vector3[NPCs.Count];
        bool isAllDestroyed = true;
        
        if (NPCLocations.Length == 0)    return new Vector3[0];

        for (int i = 0; i < NPCs.Count; i++)
        {
            if (NPCs[i] != null)
            {
                NPCLocations[i] = NPCs[i].transform.position;
                isAllDestroyed = false;
            }
        }

        if (isAllDestroyed)     return new Vector3[0];
        else return NPCLocations;
    }

    public Script_StaticNPC GetNPC(int i)
    {
        return NPCs[i];
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

    public void MovingNPCActuallyMove(int i)
    {
        movingNPCs[i].ActuallyMove();
    }

    public void SetMovingNPCExit(int i, bool shouldExit)
    {
        movingNPCs[i].shouldExit = shouldExit;
    }

    public Script_MovingNPC GetMovingNPC(int i)
    {
        return movingNPCs[i];
    }

    public void ChangeMovingNPCSpeed(int i, float speed)
    {
        movingNPCs[i].ChangeSpeed(speed);
    }

    public void CurrentMovesDoneAction()
    {
        Levels.levelsData[level].behavior.HandleMovingNPCCurrentMovesDone();
        // if (!bgMusicManager.GetIsPlaying())    UnPauseBgMusic();
        
        // if (eroBgThemePlayer != null)
        // {
        //     PauseEroTheme();
        // }
    }

    public void AllMovesDoneAction(int i)
    {
        Levels.levelsData[level].behavior.HandleMovingNPCAllMovesDone();
    }

    public void OnApproachedTarget(int i)
    {
        Levels.levelsData[level].behavior.HandleMovingNPCOnApproachedTarget(i);
    }

    public void NPCFaceDirection(int Id, string direction)
    {
        foreach(Script_MovingNPC NPC in movingNPCs)
        {
            if (NPC.MovingNPCId == Id)   NPC.FaceDirection(direction);
        }
    }

    /* =======================================================================
        _SAVEPOINTS_
    ======================================================================= */

    // only max of 1 in a Level
    public void SetupSavePoint(Script_SavePoint sp, bool isInitialize)
    {
        savePoint = sp;
        print($"savePoint: {sp}");
        savePointCreator.SetupSavePoint(sp, isInitialize);
    }

    void ClearSavePoint()
    {
        savePoint = null;
    }
    
    public Model_SavePointData GetSavePointData()
    {
        return savePoint.GetData();
    }

    /* =======================================================================
        _INTERACTABLE OBJECTS_
    ======================================================================= */
    
    // must give world offset bc IOs in Scene are parented by world
    public void SetupInteractableObjectsText(
        Transform textObjectParent,
        bool isInitialize
    )
    {
        interactableObjectCreator.SetupInteractableObjectsText(
            textObjectParent,
            interactableObjects,
            GetRotationToFaceCamera(),
            dialogueManager,
            player,
            worldOffset,
            isInitialize
        );
    }

    public void SetupLightSwitches(
        Transform lightSwitchesParent,
        bool[] switchesState,
        bool isInitialize
    )
    {
        interactableObjectCreator.SetupLightSwitches(
            lightSwitchesParent,
            interactableObjects,
            switches,
            GetRotationToFaceCamera(),
            switchesState,
            isInitialize
        );
    }

    public void SetupPushables(
        Transform parent,
        bool isInitialize
    )
    {
        interactableObjectCreator.SetupPushables(
            parent,
            interactableObjects,
            pushables,
            isInitialize
        );
    }

    public void CreateInteractableObjects(
        bool[] switchesState,
        bool isForceSortingLayer,
        bool isSortingLayerAxisZ = true,
        int offset = 0
    )
    {
        interactableObjectCreator.CreateInteractableObjects(
            Levels.levelsData[level].InteractableObjectsData,
            interactableObjects,
            switches,
            GetRotationToFaceCamera(),
            dialogueManager,
            player,
            switchesState,
            isForceSortingLayer,
            isSortingLayerAxisZ,
            offset
        );
    }

    void DestroyInteractableObjects()
    {
        interactableObjectCreator.DestroyInteractableObjects(
            interactableObjects, 
            switches,
            pushables
        );    
    }

    void ClearInteractableObjects()
    {
        interactableObjectCreator.ClearInteractableObjects(
            interactableObjects, 
            switches,
            pushables
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

    /* =======================================================================
        _DEMONS_
    ======================================================================= */

    public void CreateDemons(bool[] spawnState)
    {
        demonCreator.CreateDemons(
            spawnState,
            Levels.levelsData[level].DemonsData,
            demons
        );
    }

    public void SetupDemons(Transform demonsParent, bool[] demonsState)
    {
        demonCreator.SetupDemons(demonsParent, demonsState, demons);
    }

    void DestroyDemons()
    {
        foreach(Script_Demon d in demons)
        {
            if (d)    Destroy(d.gameObject);
        }

        demons.Clear();
    }

    void ClearDemons()
    {
        demons.Clear();
    }

    public List<Script_Demon> GetDemons()
    {
        return demons;
    }

    public void EatDemon(int Id)
    {
        demonHandler.EatDemon(Id, demons);
        levelBehavior.EatDemon(Id);
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

    /* =========================================================================
        _DIALOGUE & THOUGHTS_
    ========================================================================= */    

    void SetupDialogueManager()
    {
        dialogueManager.Setup();
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

    public void HandleDialogueNodeAction(string action)
    {
        levelBehavior.HandleDialogueNodeAction(action);
    }

    public void HandleDialogueNodeUpdateAction(string action)
    {
        levelBehavior.HandleDialogueNodeUpdateAction(action);
    }

    /* =========================================================================
        _DDR_
    ========================================================================= */

    public void HandleDDRArrowClick(int tier)
    {
        DDRHandler.HandleArrowClick(tier, levelBehavior);
    }
    
    /* =========================================================================
        _MUSIC_
    ========================================================================= */

    void StartBgMusic()
    {
        // TODO: make this a general theme player, not just for Ero
        if (npcBgThemePlayer != null && GetNPCThemeMusicIsPlaying())   return;
        
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

    public void PlayNPCBgTheme(Script_BgThemePlayer bgThemePlayerPrefab)
    {
        npcBgThemePlayer = Instantiate(
            bgThemePlayerPrefab,
            player.transform.position,
            Quaternion.identity
        );
        npcBgThemePlayer.transform.SetParent(bgThemeSpeakersContainer, false);
    }

    public void PauseNPCBgTheme()
    {
        if (npcBgThemePlayer == null)   return;
        npcBgThemePlayer.GetComponent<AudioSource>().Pause();
    }

    public void UnPauseNPCBgTheme()
    {
        if (npcBgThemePlayer == null)
        {
            Debug.Log("No npcBgThemePlayer object exists to UnPause.");
            return;
        }
        npcBgThemePlayer.GetComponent<AudioSource>().UnPause();
    }

    public void StopBgTheme()
    {
        npcBgThemePlayer.GetComponent<AudioSource>().Stop();
        Destroy(npcBgThemePlayer.gameObject);

        npcBgThemePlayer = null;
    }

    public bool GetNPCBgThemeActive()
    {
        return  npcBgThemePlayer != null;
    }

    public bool GetNPCThemeMusicIsPlaying()
    {
        return npcBgThemePlayer.GetComponent<AudioSource>().isPlaying;
    }

    public void StopMovingNPCThemes()
    {
        if (npcBgThemePlayer == null)   return;

        if (Levels.levelsData[level].shouldPersistBgThemes)    PauseNPCBgTheme();
        
        StopBgTheme();
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

    /* =========================================================================
        _CAMERA_
    ========================================================================= */

    public void CameraMatchPositionToTarget()
    {
        camera.MatchPositionToTarget();
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
        CameraMoveToTarget();
    }

    public void CameraMoveToTarget()
    {
        camera.MoveToTarget();
    }

    public void SetOrthographicSizeDefault()
    {
        camera.SetOrthographicSizeDefault();
    }

    public void SetOrthographicSize(float size)
    {
        camera.SetOrthographicSize(size);
    }

    public void CameraZoomSmooth(float size, float time, Vector3 loc, Action cb)
    {
        camera.ZoomSmooth(size, time, loc, cb);
    }

    public void CameraMoveToTargetSmooth(float time, Vector3 loc, Action cb)
    {
        camera.MoveToTargetSmooth(time, loc, cb);
    }

    public void CameraSetIsTrackingTarget(bool isTracking)
    {
        camera.SetIsTrackingTarget(isTracking);
    }

    public void SetCameraOffset(Vector3 offset)
    {
        camera.SetOffset(offset);
    }

    public void SetCameraOffsetDefault()
    {
        camera.SetOffsetToDefault();
    }
    
    public void CameraInstantMoveSpeed()
    {
        camera.InstantTrackSpeed();
    }

    public void CameraDefaultMoveSpeed()
    {
        camera.DefaultSpeed();
    }

    public Vector3 GetRotationToFaceCamera()
    {
        return camera.GetRotationAdjustment();
    }

    /* =========================================================================
        _EXITS_
    ========================================================================= */
    public void DisableExits(bool isDisabled, int i)
    {
        print("game.DisableExits()=================");
        exitsHandler.DisableExits(isDisabled, i);
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

    public void HandleExitCutSceneLevelBehavior()
    {
        levelBehavior.HandleExitCutScene();
    }

    public void OnDoorLockUnlock(int id)
    {
        levelBehavior.OnDoorLockUnlock(id);
    }

    /* =========================================================================
        _SCENES_
    ========================================================================= */    

    public void SwitchSceneToTitle()
    {
        sceneManager.SwitchSceneToTitle();
    }

    /* =========================================================================
        _CUTSCENES_
    ========================================================================= */    
    public void MelanholicTitleCutScene()
    {
        cutSceneManager.MelanholicTitleCutScene();
    }   
}
