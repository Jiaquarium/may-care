using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_PushableCheckCollisions))]
[RequireComponent(typeof(Script_InteractionBoxController))]
public class Script_Pushable : Script_InteractableObject
{
    public int pushableId;
    public float speed;
    [SerializeField] private float progress;
    [SerializeField] private bool isMoving;
    [SerializeField] private Vector3 startLocation;
    [SerializeField] private Vector3 endLocation;
    
    void FixedUpdate()
    {
        if (isMoving)   ActuallyMove();
    }
    
    public void Push(string dir)
    {
        if (isMoving)   return;
        
        Vector3 desiredDir = Script_Utils.GetDirectionToVectorDict()[dir];
        // check for collisions
        // if no collisions then able to push, return true
        GetComponent<Script_InteractionBoxController>().HandleActiveInteractionBox(dir);
        bool isCollision = GetComponent<Script_CheckCollisions>().CheckCollisions(
            transform.position, dir
        );
        if (isCollision)
        {
            return;
        }

        startLocation = transform.position;
        endLocation = transform.position + desiredDir;
        Move();
    }

    void Move()
    {
        progress = 0f;
        isMoving = true;
    }

    void ActuallyMove()
    {
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(startLocation, endLocation, progress);

        if (progress >= 1f)
        {
            progress = 1f;
            isMoving = false;
        }
    }
}
