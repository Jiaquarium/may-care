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
                && objects[i].isActive
            )
            {
                // space
                if (action == "Action1")
                {
                    objects[i].ActionDefault();
                }
                // enter
                else if (action == "Submit")
                {
                    objects[i].ActionB();
                }

                return true;
            }
        }

        return false;
    }

    public Vector3[] GetLocations(List<Script_InteractableObject> objs)
    {
        if (objs.Count == 0)    return new Vector3[0];
        
        Vector3[] objLocations = new Vector3[objs.Count];

        for (int i = 0; i < objs.Count; i++)
        {
            objLocations[i] = objs[i].transform.position;
        }

        return objLocations;
    }
}
