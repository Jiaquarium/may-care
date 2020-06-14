using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[RequireComponent(typeof(Script_PlayerCheckCollisions))]
public class Script_PlayerMovement : MonoBehaviour
{
    public Script_PlayerGhost PlayerGhostPrefab;
    public Script_PlayerReflection PlayerReflectionPrefab;


    public float repeatDelay;
    public int exitUpStairsOrderLayer;
    

    private Script_Game game;
    private Script_Player player;
    private Script_PlayerGhost playerGhost;
    private Script_PlayerReflection playerReflection;
    private Dictionary<string, Vector3> Directions;
    private SpriteRenderer spriteRenderer;
    private Vector3[] NPCLocations = new Vector3[0];
    private Vector3[] DemonLocations = new Vector3[0];
    private Vector3[] InteractableObjectLocations = new Vector3[0];
    private bool isMoving;
    private Transform grid;


    public float progress;
    
    
    public string lastMove;
    public float timer;

    void OnDestroy() {
        if (playerGhost != null)    Destroy(playerGhost.gameObject);
    }

    public void HandleMoveInput()
    {
        timer = Mathf.Max(0f, timer - Time.deltaTime);
        
        SetMoveAnimation();
        
        Vector2 dirVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (dirVector == Vector2.zero)  return;
        
        // determine if vector is up, down, left or right direction headed
        if (
            Mathf.Abs(dirVector.x) > Mathf.Abs(dirVector.y)
            && dirVector.x > 0
        )    
        {
            Move("right");
        }
        else if (Mathf.Abs(dirVector.x) > Mathf.Abs(dirVector.y) && dirVector.x < 0)
        {
            Move("left");
        }
        else if (Mathf.Abs(dirVector.y) > Mathf.Abs(dirVector.x) && dirVector.y > 0)
        {
            Move("up");
        }
        else if (Mathf.Abs(dirVector.y) > Mathf.Abs(dirVector.x) && dirVector.y < 0)
        {
            Move("down");
        }
    }

    void SetMoveAnimation()
    {
        // move animation when direction button down 
        player.animator.SetBool(
            "PlayerMoving",
            Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f
        );

        playerGhost.SetMoveAnimation();
    }

    bool CheckRepeatMove(string dir)
    {
        if (timer == 0.0f)
        {   
            return true;
        }

        return false;
    }

    void Move(string dir)
    {
        if (dir == lastMove)
        {
            if (!CheckRepeatMove(dir)) return;
        }

        Vector3 desiredDirection = Directions[dir];
        
        player.AnimatorSetDirection(dir);
        playerGhost.AnimatorSetDirection(dir);
        PushPushables(dir);

        if (CheckCollisions(dir))  return;

        /*
            in DDR mode, only changing directions to look like dancing
        */
        if (game.state == "ddr")    return;
        
        progress = 0f;
        timer = repeatDelay;

        /*
            move player to desired loc, and start playerGhost's animation
            after-the-fact
        */
        player.startLocation = player.location;
        playerGhost.startLocation = player.location;

        player.location += desiredDirection;
        playerGhost.location += desiredDirection;
        // move player pointer immediately
        transform.position = player.location;
        HandleMoveAnimation(dir);

        lastMove = dir;
    }

    void HandleMoveAnimation(string dir)
    {
        isMoving = true;
        playerGhost.Move(dir);
        spriteRenderer.enabled = false;
    }

    bool CheckCollisions(string dir)
    {   
        // if reflection is interactive check if it can move; if not, disallow player from moving as well
        if (playerReflection is Script_PlayerReflectionInteractive)
        {
            if (!playerReflection.GetComponent<Script_PlayerReflectionInteractive>().CanMove())
                return true;
        }
        
        return GetComponent<Script_PlayerCheckCollisions>().CheckCollisions(
            player.location, dir
        );
    }

    void PushPushables(string dir)
    {
        player.TryPushPushable(dir); // push needs to come before collision detection
        
        if (playerReflection is Script_PlayerReflectionInteractive)
        {
            playerReflection.GetComponent<Script_PlayerReflectionInteractive>().TryPushPushable(dir);
        }
    }

    public void TrackPlayerGhost()
    {
        progress = playerGhost.progress;
        
        if (progress >= 1f && isMoving)
        {
            FinishMoveAnimation();
        }
    }

    void FinishMoveAnimation()
    {
        // must be visible before playerghost invisible to avoid flicker
        spriteRenderer.enabled = true;
        playerGhost.SetIsNotMoving();
        isMoving = false;
    }

    public void HandleExitTile()
    {
        Tilemap entrancesTileMap = game.GetEntrancesTileMap();
        Tilemap[] exitsTileMaps = game.GetExitsTileMaps();
        
        Vector3Int tileLocation = new Vector3Int(
            (int)Mathf.Round(player.location.x - grid.position.x), // *adjust for grid offset*
            (int)Mathf.Round(player.location.z - grid.position.z),
            0
        );

        if (exitsTileMaps != null && exitsTileMaps.Length > 0)
        {
            foreach(Tilemap tm in exitsTileMaps)
            {
                if (tm.HasTile(tileLocation))
                {
                    Script_TileMapExitEntrance exitInfo = tm.GetComponent<Script_TileMapExitEntrance>();
                    
                    if (exitInfo.type == Script_ExitTypes.StairsUp)
                    {
                        HandleStairsExitAnimation();
                    }

                    if (exitInfo.type == Script_ExitTypes.ExitCutSceneLB)
                    {
                        game.HandleExitCutSceneLevelBehavior();
                        return;
                    }                    
                    
                    game.Exit(
                        exitInfo.level,
                        exitInfo.playerNextSpawnPosition,
                        exitInfo.playerFacingDirection,
                        true
                    );
                    return;
                }
            }
        }

        if (entrancesTileMap != null && entrancesTileMap.HasTile(tileLocation))
        {
            Script_TileMapExitEntrance entranceInfo = entrancesTileMap.GetComponent<Script_TileMapExitEntrance>();
            
            if (entranceInfo.type == Script_ExitTypes.StairsUp)
            {
                HandleStairsExitAnimation();
            }

            game.Exit(
                entranceInfo.level,
                entranceInfo.playerNextSpawnPosition,
                entranceInfo.playerFacingDirection,
                false
            );
            return;
        }
    }

    void HandleStairsExitAnimation()
    {
        Script_Utils.FindComponentInChildWithTag<SpriteRenderer>(
            this.gameObject, Const_Tags.PlayerAnimator
        ).sortingOrder = exitUpStairsOrderLayer;
        player.PlayerGhostMatchSortingLayer();
    }

    Script_PlayerGhost CreatePlayerGhost(bool isLightOn)
    {
        Script_PlayerGhost pg = Instantiate(
            PlayerGhostPrefab,
            player.transform.position,
            Quaternion.identity
        );

        pg.SwitchLight(isLightOn);

        return pg;
    }

    public void PlayerGhostSortOrder(int sortingOrder)
    {
        playerGhost.spriteRenderer.sortingOrder = sortingOrder;
    }

    public Script_PlayerReflection CreatePlayerReflection(Vector3 reflectionAxis)
    {
        Script_PlayerReflection pr = Instantiate(
            PlayerReflectionPrefab,
            player.transform.position, // will update within Script_PlayerReflection
            Quaternion.identity
        );
        pr.Setup(
            playerGhost,
            player,
            reflectionAxis
        );
        pr.transform.SetParent(game.playerContainer, false);
        
        SetPlayerReflection(pr);
        return pr;
    }

    public void SetPlayerReflection(Script_PlayerReflection pr)
    {
        playerReflection = pr;
    }

    public void RemoveReflection()
    {
        if (playerReflection != null)
        {
            Destroy(playerReflection.gameObject);
        }
    }

    public Script_PlayerGhost GetPlayerGhost()
    {
        return playerGhost;
    }

    public void SwitchLight(bool isOn)
    {
        playerGhost.SwitchLight(isOn);
    }
    
    public void InitializeOnLevel(Transform _grid)
    {
        playerGhost.Setup(player.transform.position);
        grid = _grid;
    }

    public void Setup(Script_Game _game, bool isLightOn)
    {
        game = _game;
        player = GetComponent<Script_Player>();
        
        // setup ghost for smooth movement (allows cancellation of mid-animation)
        playerGhost = CreatePlayerGhost(isLightOn);
        playerGhost.Setup(player.transform.position);
        playerGhost.transform.SetParent(game.playerContainer, false);
        
        spriteRenderer = Script_Utils.FindComponentInChildWithTag<SpriteRenderer>(
            this.gameObject, Const_Tags.PlayerAnimator
        );
        Script_Utils.FindComponentInChildWithTag<Script_PlayerMovementAnimator>(
            this.gameObject, Const_Tags.PlayerAnimator
        ).Setup();

        Directions = Script_Utils.GetDirectionToVectorDict();

        timer = repeatDelay;
        progress = 1f;
    }
}
