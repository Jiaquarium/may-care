using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Script_PlayerMovement : MonoBehaviour
{
    // public AnimationCurve progressCurve;
    public Script_PlayerGhost PlayerGhostPrefab;

    
    public float speed;
    public float repeatDelay;
    

    private Script_Game game;
    private Script_Player player;
    private Script_PlayerGhost playerGhost;
    private Dictionary<string, Vector3> Directions;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    private SpriteRenderer spriteRenderer;
    
    
    private Vector3[] MovingNPCLocations = new Vector3[0];
    private Vector3[] DemonLocations = new Vector3[0];
    private bool isMoving;


    // TODO: make private
    public float progress;
    
    
    public string lastMove;
    public float timer;

    public void HandleMoveInput(bool playerIsTalking)
    {
        TrackPlayerGhost();

        // finish movement when starting conversation
        if (playerIsTalking)
        {
            return;
        }

        timer = Mathf.Max(0f, timer - Time.deltaTime);
        
        SetMoveAnimation();
        
        Vector2 dirVector = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
        // TODO: do we need a timer for every direction?
        if (timer == 0.0f)
        {   
            // timer = repeatDelay;
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

        player.startLocation = player.location;
        playerGhost.startLocation = player.location;

        // location updates immediately, do we need?? 
        player.location += desiredDirection;
        // this will lag
        playerGhost.location += desiredDirection;
        
        HandleMoveAnimation(dir);

        // actually begin to move
        // player.localState = "move";
        // player.localState = "interact";
        transform.position = player.location;
        lastMove = player.facingDirection;
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
        
        // tiles map from (xyz) to (xz)
        if (
            !tileMap.HasTile(new Vector3Int(desiredX, desiredZ, 0))
            && !exitsTileMap.HasTile(new Vector3Int(desiredX, desiredZ, 0))
        ) 
        {
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

        return false;
    }

    void TrackPlayerGhost()
    {
        progress = playerGhost.progress;
        
        // begin ghost movement
        // turn this sprite not visible
        // turn ghost sprite visible

        // progress += speed;
        // transform.position = Vector3.Lerp(
        //     player.startLocation,
        //     player.location,
        //     progressCurve.Evaluate(progress)
        // );
        
        if (progress >= 1f && isMoving)
        {
            FinishMoveAnimation();
            HandleExitTile();
        }
    }

    public void FinishMoveAnimation()
    {
        // must be visible before playerghost invisible to avoid flicker
        spriteRenderer.enabled = true;
        playerGhost.SetIsNotMoving();
        isMoving = false;
    }

    void HandleExitTile()
    {
        if (exitsTileMap.HasTile(
            new Vector3Int(
                (int)Mathf.Round(player.location.x),
                (int)Mathf.Round(player.location.z),
                0
            )
        ))
        {
            game.HandleLevelExit();
        }
    }

    Script_PlayerGhost CreatePlayerGhost()
    {
        return Instantiate(
            PlayerGhostPrefab,
            player.transform.position,
            Quaternion.identity
        );
    }
    
    public void Setup(
        Script_Game _game,
        Dictionary<string, Vector3> _Directions,
        Tilemap _tilemap,
        Tilemap _exitsTileMap
    )
    {
        player = GetComponent<Script_Player>();
        playerGhost = CreatePlayerGhost();
        playerGhost.Setup(player.transform.position);
        spriteRenderer = GetComponent<SpriteRenderer>();

        game = _game;
        Directions = _Directions;
        tileMap = _tilemap;
        exitsTileMap = _exitsTileMap;

        timer = repeatDelay;
        progress = 1f;
    }
}
