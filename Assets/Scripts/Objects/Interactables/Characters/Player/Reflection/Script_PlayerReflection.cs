using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_PlayerReflectionMovement))]
public class Script_PlayerReflection : MonoBehaviour
{
    public Vector3 axis;
    private Script_PlayerReflectionMovement reflectionMovement;
    private Script_Player player;
    [SerializeField] Transform graphics;
    
    void Update()
    {
        reflectionMovement.HandleMove();
    }

    public string ToOppositeDirectionZ(string desiredDir)
    {
        if      (desiredDir == "right")     return "right";
        else if (desiredDir == "left")      return "left";
        else if (desiredDir == "up")        return "down";
        else                                return "up";
    }

    public void AdjustRotation()
    {
        graphics.transform.forward = Camera.main.transform.forward;
    }
    
    public void Setup(
        Script_PlayerGhost _playerGhost,
        Script_Player _player,
        Vector3 _axis
    )
    {
        reflectionMovement = GetComponent<Script_PlayerReflectionMovement>();
        reflectionMovement.Setup(this, _playerGhost, _player, _axis);
        
        player = _player;
        axis = _axis;

        transform.position = reflectionMovement.GetReflectionPosition(player.transform.position);

        AdjustRotation();
    }
}
