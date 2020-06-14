using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Script_InteractableBox : MonoBehaviour
{
    [SerializeField] Vector3 boxSize; // half extants
    [SerializeField] Color color;

    private void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2)); // size is halfExtents
    }
}
