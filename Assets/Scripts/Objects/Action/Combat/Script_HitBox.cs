using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitBoxResponder
{
    void CollisionedWith(Collider collider);
}

public class Script_HitBox : MonoBehaviour
{
    [SerializeField] protected Collider[] colliders;
    [SerializeField] private Vector3 boxSize; // half extants
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color collisionOpenColor;
    [SerializeField] private Color collidingColor;
    [SerializeField] private ColliderState state;
    private IHitBoxResponder responder = null;
    
    void Update()
    {
        if (state == ColliderState.Closed)  return;
        ExposeBox();
        CheckColliding();
    }
    
    void ExposeBox()
    {
        colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask);
    }

    void CheckColliding()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider aCollider = colliders[i];
            responder?.CollisionedWith(aCollider);
        }

        state = colliders.Length > 0 ? state = ColliderState.Colliding : state = ColliderState.Open;
    }

    public void SetResponder(IHitBoxResponder _responder)
    {
        responder = _responder;
    }

    void OnDrawGizmos() {
        CheckGizmoColor();
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2)); // size is halfExtents
    }

    public void StartCheckingCollision() {
        state = ColliderState.Open;
    }

    public void StopCheckingCollision() {
        state = ColliderState.Closed;
    }

    private void CheckGizmoColor() {
        switch(state) {
            case ColliderState.Closed:
                Gizmos.color = inactiveColor;
                break;
            case ColliderState.Open:
                Gizmos.color = collisionOpenColor;
                break;
            case ColliderState.Colliding:
                Gizmos.color = collidingColor;
                break;
        }
    }
}