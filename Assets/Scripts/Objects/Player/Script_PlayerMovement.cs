using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Script_PlayerMovement : MonoBehaviour
{
    public AnimationCurve progressCurve;
    public float speed;
    public float repeatDelay;
    

    private Script_Game game;
    private Script_Player player;
    private Dictionary<string, Vector3> Directions;
    private Tilemap tileMap;
    private Tilemap exitsTileMap;
    private Vector3[] MovingNPCLocations = new Vector3[0];
    private Vector3[] DemonLocations = new Vector3[0];


    // TODO: make private
    public float progress;
    
    
    public string lastMove;
    public float timer;

    public void HandleMoveInput()
    {
        ActuallyMove();
        
        timer = Mathf.Max(0f, timer - Time.deltaTime);

        Vector2 dirVector = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        // restrict input until LERP animation is done
        if (player.localState == "move")    return;

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
        player.location += desiredDirection;
        
        // actually begin to move
        player.localState = "move";
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

    void ActuallyMove()
    {
        progress += speed;
        transform.position = Vector3.Lerp(
            player.startLocation,
            player.location,
            progressCurve.Evaluate(progress)
        );
        
        if (progress >= 1f)
        {
            FinishMoveAnimation();
        }
    }

    void FinishMoveAnimation()
    {
        player.localState = "interact";
        progress = 1f;
        transform.position = player.location;
        lastMove = player.facingDirection;

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
    
    public void Setup(
        Script_Game _game,
        Dictionary<string, Vector3> _Directions,
        Tilemap _tilemap,
        Tilemap _exitsTileMap
    )
    {
        player = GetComponent<Script_Player>();

        game = _game;
        Directions = _Directions;
        tileMap = _tilemap;
        exitsTileMap = _exitsTileMap;

        timer = repeatDelay;
        progress = 1f;
    }
}
