using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Script_PlayerMovement : MonoBehaviour
{
    public Script_PlayerGhost PlayerGhostPrefab;


    public float repeatDelay;
    

    private Script_Game game;
    private Script_Player player;
    private Script_PlayerGhost playerGhost;
    private Dictionary<string, Vector3> Directions;
    private SpriteRenderer spriteRenderer;
    
    
    private Vector3[] MovingNPCLocations = new Vector3[0];
    private Vector3[] DemonLocations = new Vector3[0];
    private Vector3[] InteractableObjectLocations = new Vector3[0];
    private bool isMoving;


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
        
        if (CheckCollisions(desiredDirection))  return;
        
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

    bool CheckCollisions(Vector3 desiredDirection)
    {   
        int desiredX = (int)Mathf.Round((player.location + desiredDirection).x);
        int desiredZ = (int)Mathf.Round((player.location + desiredDirection).z);
        
        Vector3Int tileLocation = new Vector3Int(desiredX, desiredZ, 0);
        
        Tilemap tileMap = game.GetTileMap();
        Tilemap entrancesTileMap = game.GetEntrancesTileMap();
        Tilemap exitsTileMap = game.GetExitsTileMap();

        // tiles map from (xyz) to (xz)
        if (!tileMap.HasTile(tileLocation))
        {
            // check to see if desiredTile is in exit/entrance tilemaps
            if (
                (
                    exitsTileMap == null
                    || !exitsTileMap.HasTile(tileLocation)
                    // if exits are disabled, ignore if found in exits tilemap
                    || game.GetIsExitsDisabled()
                )
                &&
                (
                    entrancesTileMap == null
                    || !entrancesTileMap.HasTile(tileLocation)
                )
            )
            return true;
        }

        // if NPC is moving check if NPC is occupying space          
        // don't check nonmoving NPCs b/c we do that in tileMap and they're static
        MovingNPCLocations = game.GetMovingNPCLocations();
        if (MovingNPCLocations.Length != 0)
        {
            foreach (Vector3 loc in MovingNPCLocations)
            {
                if (desiredX == loc.x && desiredZ == loc.z) return true;    
            }
        }

        // if Demons on map check if occupying space
        DemonLocations = game.GetDemonLocations();
        if (DemonLocations.Length != 0)
        {
            foreach (Vector3 loc in DemonLocations)
            {
                if (desiredX == loc.x && desiredZ == loc.z) return true;
            }
        }

        InteractableObjectLocations = game.GetInteractableObjectLocations();
        if (InteractableObjectLocations.Length != 0)
        {
            foreach (Vector3 loc in InteractableObjectLocations)
            {
                if (desiredX == loc.x && desiredZ == loc.z) return true;
            }
        }

        return false;
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
        Tilemap exitsTileMap = game.GetExitsTileMap();
        
        Vector3Int tileLocation = new Vector3Int(
            (int)Mathf.Round(player.location.x),
            (int)Mathf.Round(player.location.z),
            0
        );

        if (exitsTileMap != null && exitsTileMap.HasTile(tileLocation))
        {
            Script_TileMapExitEntrance exitInfo = exitsTileMap.GetComponent<Script_TileMapExitEntrance>();
            game.Exit(
                exitInfo.level,
                exitInfo.playerNextSpawnPosition,
                exitInfo.playerFacingDirection,
                true
            );
            return;
        }

        if (entrancesTileMap != null && entrancesTileMap.HasTile(tileLocation))
        {
            Script_TileMapExitEntrance entranceInfo = entrancesTileMap.GetComponent<Script_TileMapExitEntrance>();
            game.Exit(
                entranceInfo.level,
                entranceInfo.playerNextSpawnPosition,
                entranceInfo.playerFacingDirection,
                false
            );
            return;
        }
    }

    Script_PlayerGhost CreatePlayerGhost(bool withLight)
    {
        Script_PlayerGhost pg = Instantiate(
            PlayerGhostPrefab,
            player.transform.position,
            Quaternion.identity
        );
        
        if (withLight)  pg.TurnLightOn();

        return pg;
    }
    
    public void Setup(
        Script_Game _game,
        Dictionary<string, Vector3> _Directions,
        bool isLightOn
    )
    {
        player = GetComponent<Script_Player>();
        playerGhost = CreatePlayerGhost(isLightOn);
        playerGhost.Setup(player.transform.position);
        spriteRenderer = GetComponent<SpriteRenderer>();

        game = _game;
        Directions = _Directions;

        timer = repeatDelay;
        progress = 1f;
    }
}
