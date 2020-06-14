using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_HurtBox : MonoBehaviour
{
    public BoxCollider myCollider;
    [SerializeField] Script_CharacterStats stats;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color collisionOpenColor;
    [SerializeField] private Color collidingColor;
    
    [SerializeField] private ColliderState state = ColliderState.Open;

    void Update()
    {
    }
    
    public void Hurt(int dmg)
    {
        stats.Hurt(dmg);

        // consider doing something with state, closing hurtbox?
    }


    private void OnDrawGizmos() {
        // same as hitbox, but taking the size, rotation and scale from the collider
        CheckGizmoColor();
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(myCollider.size.x, myCollider.size.y, myCollider.size.z)); // size is halfExtents
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
