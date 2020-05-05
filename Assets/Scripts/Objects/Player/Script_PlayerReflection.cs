using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerReflection : MonoBehaviour
{
    public AnimationCurve progressCurve;
    
    
    private Animator animator;
    public Script_PlayerGhost playerGhost;
    public Script_Player player;


    public Vector3 reflectionAxis;


    public float speed;
    public Vector3 startLocation;
    public Vector3 location;
    public float progress;
    public bool isMoving;

    
    void Update()
    {
        HandlePlayerMoves();
        SetMoveAnimation();
        if (isMoving)    ActuallyMove();
    }

    void ActuallyMove()
    {
        progress += speed;
        transform.position = Vector3.Lerp(
            startLocation,
            location,
            progressCurve.Evaluate(progress)
        );

        if (progress >= 1f)
        {
            progress = 1f;
        }
    }

    public void Move(string dir)
    {
        Script_Utils.AnimatorSetDirection(animator, dir);
        progress = 0f;
    }

    void SetMoveAnimation()
    {
        animator.SetBool(
            "NPCMoving",
            Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f
        );
    }

    string ToOppositeDirectionZ(string desiredDir)
    {
        if      (desiredDir == "right")     return "right";
        else if (desiredDir == "left")      return "left";
        else if (desiredDir == "up")        return "down";
        else                                return "up";
    }

    void HandlePlayerMoves()
    {
        startLocation = GetReflectionPosition(playerGhost.transform.position);
        location = GetReflectionPosition(player.transform.position);

        Move(ToOppositeDirectionZ(player.facingDirection));

        // decide if reflection should physically move
        if (location != startLocation)
        {
            SetIsActuallyMoving();
        }
        else
        {
            SetIsActuallyNotMoving();
        }
    }
    
    void SetIsActuallyMoving()
    {
        isMoving = true;
    }

    void SetIsActuallyNotMoving()
    {
        isMoving = false;
    }

    Vector3 GetReflectionPosition(Vector3 loc)
    {
        // TODO: currently only works for reflection to be on top and to the right
        float reflectedZ = reflectionAxis.z + Mathf.Abs(reflectionAxis.z - loc.z);
        float reflectedX = reflectionAxis.x + Mathf.Abs(reflectionAxis.x - loc.x) + 1.0f;

        return new Vector3(reflectedX, loc.y, reflectedZ);
    }

    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }
    
    public void Setup(
        Script_PlayerGhost _playerGhost,
        Script_Player _player,
        Vector3 _reflectionAxis
    )
    {
        print("SETUP IN PLAYERREFLECTION, _playerGhost: " + _playerGhost);
        print("SETUP IN PLAYERREFLECTION, _player: " + _player);
        animator = GetComponent<Animator>();
        
        // get axis to reflect on based on playerLoc

        playerGhost = _playerGhost;
        player = _player;
        reflectionAxis = _reflectionAxis;
        location = GetReflectionPosition(player.transform.position);
        transform.position = location;

        progress = 1f;

        AdjustRotation();
    }
}
