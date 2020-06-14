using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PushablesCreator : MonoBehaviour
{
    public void SetupPushables(
        Transform pushablesParent,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Pushable> pushables,
        bool isInit
    )
    {
        for (int i = 0; i < pushablesParent.childCount; i++)
        {
            Script_Pushable p = pushablesParent.GetChild(i).GetComponent<Script_Pushable>();
            interactableObjects.Add(p);
            pushables.Add(p);

            if (isInit)
            {
                p.Id = interactableObjects.Count - 1;
                p.pushableId = i;
            }
        }
    }
}
