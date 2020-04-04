using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectCreator : MonoBehaviour
{
    public Script_InteractableObject InteractableObjectPrefab;
    public Script_Switch SwitchPrefab;
    public Script_LightSwitch LightSwitchPrefab;
    
    private Light[] lights;


    public float defaultOnIntensity;
    public float defaultOffIntensity;

    public void CreateInteractableObjects(
        Model_InteractableObject[] interactableObjectsData,
        List<Script_InteractableObject> interactableObjects,
        Vector3 rotationAdjustment
    )
    {
        Script_InteractableObject iObj;
        
        if (interactableObjectsData.Length == 0)    return;

        for (int i = 0; i < interactableObjectsData.Length; i++)
        {
            if (interactableObjectsData[i].type == "lightswitch")
            {
                iObj = Instantiate(
                    LightSwitchPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
                
                lights = interactableObjectsData[i].lights;

                // if didn't customize, then use default
                float onIntensity = interactableObjectsData[i].lightOnIntensity;
                float offIntensity = interactableObjectsData[i].lightOffIntensity;
                if (onIntensity == 0f && offIntensity == 0)
                {
                    onIntensity = defaultOnIntensity;
                    offIntensity = defaultOffIntensity;
                }
                iObj.SetupLights(lights, onIntensity, offIntensity);
            }
            else if (interactableObjectsData[i].type == "switch")
            {
                iObj = Instantiate(
                    SwitchPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
            } else
            {
                iObj = Instantiate(
                    InteractableObjectPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
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
