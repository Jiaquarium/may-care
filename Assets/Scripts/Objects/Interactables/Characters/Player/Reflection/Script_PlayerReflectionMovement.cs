using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerReflectionMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float xOffset;
    [SerializeField] private float zOffset;
    protected Script_Player player;
    private Script_PlayerReflection playerReflection;
    private Script_PlayerGhost playerGhost;
    private Vector3 axis;
    
    public void HandleMove()
    {
        MoveAnimation(player.facingDirection);
        ActuallyMove();
    }
    protected virtual void ActuallyMove()
    {
        if (playerGhost == null)    return;
        
        // allows to reflect player when first spawned on level and not moving
        if (!playerGhost.spriteRenderer.enabled)
            transform.position = GetReflectionPosition(player.transform.position);
        else transform.position = GetReflectionPosition(playerGhost.transform.position);
    }

    void MoveAnimation(string dir)
    {
        string myFaceDirection = ToOppositeDirectionZ(dir);
        Script_Utils.AnimatorSetDirection(animator, myFaceDirection);
        animator.SetBool(
            "NPCMoving",
            Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f
        );
    }

    protected string ToOppositeDirectionZ(string desiredDir)
    {
        return playerReflection.ToOppositeDirectionZ(desiredDir);   
    }
    
    public Vector3 GetReflectionPosition(Vector3 loc)
    {
        // TODO: currently only works for reflection to be on top and to the right
        float reflectedZ = axis.z + Mathf.Abs(axis.z - loc.z) + zOffset;
        float reflectedX = axis.x + Mathf.Abs(axis.x - loc.x) + xOffset;

        return new Vector3(reflectedX, loc.y, reflectedZ);
    }

    public void Setup(
        Script_PlayerReflection _playerReflection,
        Script_PlayerGhost _playerGhost,
        Script_Player _player,
        Vector3 _axis
    )
    {
        playerGhost = _playerGhost;
        player = _player;
        axis = _axis;
        playerReflection = _playerReflection;
    }
}
