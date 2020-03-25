using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectHandler : MonoBehaviour
{
    public bool HandleAction(
        List<Script_InteractableObject> objects,
        Vector3 desiredLocation,
        string action
    )
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (
                desiredLocation.x == objects[i].transform.position.x
                && desiredLocation.z == objects[i].transform.position.z
            )
            {
                if (action == "Action1")
                {
                    objects[i].DefaultAction();
                }

                return true;
            }
        }

        return false;
    }
}
