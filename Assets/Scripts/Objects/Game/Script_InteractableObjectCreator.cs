using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectCreator : MonoBehaviour
{
    public Script_InteractableObject InteractableObjectPrefab;
    public Script_Switch SwitchPrefab;
    public Script_LightSwitch LightSwitchPrefab;
    public Script_InteractableObjectText InteractableObjectTextPrefab;
    
    private Light[] lights;
    private Sprite OnSprite;
    private Sprite OffSprite;


    public float defaultOnIntensity;
    public float defaultOffIntensity;

    public void CreateInteractableObjects(
        Model_InteractableObject[] interactableObjectsData,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches,
        Vector3 rotationAdjustment,
        Script_DialogueManager dialogueManager,
        Script_Player player,
        bool[] switchesState
    )
    {
        if (interactableObjectsData.Length == 0)    return;

        for (int i = 0; i < interactableObjectsData.Length; i++)
        {
            if (interactableObjectsData[i].type == "text")
            {
                Script_InteractableObjectText iObj;

                iObj = Instantiate(
                    InteractableObjectTextPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
                
                iObj.SetupText(dialogueManager, player, interactableObjectsData[i].dialogue);
                interactableObjects.Add(iObj);
                iObj.Id = i;
                iObj.Setup();
            }
            else if (interactableObjectsData[i].type == "lightswitch")
            {
                Script_LightSwitch iObj;
                
                iObj = Instantiate(
                    LightSwitchPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
                
                lights = interactableObjectsData[i].lights;
                OnSprite = interactableObjectsData[i].onSprite;
                OffSprite = interactableObjectsData[i].offSprite;

                // if didn't customize, then use default
                float onIntensity = interactableObjectsData[i].lightOnIntensity;
                float offIntensity = interactableObjectsData[i].lightOffIntensity;
                if (onIntensity == 0f && offIntensity == 0)
                {
                    onIntensity = defaultOnIntensity;
                    offIntensity = defaultOffIntensity;
                }
                // TODO 
                interactableObjects.Add(iObj);
                switches.Add(iObj);
                iObj.Id = i;
                iObj.switchId = switches.Count - 1;
                iObj.SetupLights(
                    lights,
                    onIntensity,
                    offIntensity,
                    switchesState == null
                        ? interactableObjectsData[i].isOn
                        : switchesState[switches.Count - 1],
                    OnSprite,
                    OffSprite
                );
                iObj.Setup();
            }
            else if (interactableObjectsData[i].type == "switch")
            {
                Script_Switch iObj;

                iObj = Instantiate(
                    SwitchPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
                // TODO 
                interactableObjects.Add(iObj);
                switches.Add(iObj);
                iObj.Id = i;
                iObj.switchId = switches.Count - 1;
                iObj.SetupSwitch(
                    switchesState == null        
                        ? interactableObjectsData[i].isOn
                        : switchesState[switches.Count - 1],
                    OnSprite,
                    OffSprite
                );
                iObj.Setup();
            } else
            {
                Script_InteractableObject iObj;
                
                iObj = Instantiate(
                    InteractableObjectPrefab,
                    interactableObjectsData[i].objectSpawnLocation,
                    Quaternion.Euler(rotationAdjustment)
                );
                interactableObjects.Add(iObj);
                iObj.Id = i;
                iObj.Setup();
            }
        }
    }

    public void DestroyInteractableObjects(
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches
    )
    {
        foreach(Script_InteractableObject io in interactableObjects)
        {
            if (io)    Destroy(io.gameObject);
        }    

        interactableObjects.Clear();
        switches.Clear();
    }
}
