using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exposed box to detect interactable box (hurtBox)
/// handles all interactions on this layer (e.g. talking, pushing)
/// </summary>
public class Script_InteractionBox : MonoBehaviour
{
    public bool isExposed;
    [SerializeField] protected Collider[] colliders;
    [SerializeField] private Vector3 boxSize; // half extants
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color color;
    protected virtual void Update()
    {
        if (isExposed)  ExposeBox();
    }
    
    protected void ExposeBox()
    {
        colliders = Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask);
    }

    private void OnDrawGizmos() {
        if (!isExposed)     return;
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2)); // size is halfExtents
    }

    public List<Script_Interactable> GetInteractables()
    {
        List<Script_Interactable> interactables = new List<Script_Interactable>();
        
        ExposeBox();

        foreach (Collider col in colliders)
        {
            if (col.transform.parent.GetComponent<Script_Interactable>() != null)
            {
                interactables.Add(col.GetComponent<Script_Interactable>());
            }
        }

        return interactables;
    }

    public virtual Script_SavePoint GetSavePoint() { return null; }
    public virtual Script_Pushable GetPushable() {
        print("DEFAULT GetPushable() Called");
        return null; 
    }
}
