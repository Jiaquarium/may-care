using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectCreator : MonoBehaviour
{
    public Script_InteractableObject InteractableObjectPrefab;
    public Script_Switch SwitchPrefab;
    
    public void CreateInteractableObjects(
        Model_InteractableObject[] interactableObjectsData,
        List<Script_InteractableObject> interactableObjects
    )
    {
        Script_InteractableObject iObj;
        
        if (interactableObjectsData.Length == 0)    return;

        for (int i = 0; i < interactableObjectsData.Length; i++)
        {
            
            if (interactableObjectsData[i].type == "switch")
            {
                iObj = Instantiate(
                    SwitchPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.identity
                );
            } else
            {
                iObj = Instantiate(
                    InteractableObjectPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.identity
                );
            }

            interactableObjects.Add(iObj);
            iObj.Id = i;
            iObj.Setup();
        }
    }

    public void DestroyInteractableObjects(
        List<Script_InteractableObject> interactableObjects    
    )
    {
        foreach(Script_InteractableObject io in interactableObjects)
        {
            if (io)    Destroy(io.gameObject);
        }    

        interactableObjects.Clear();
    }
}
