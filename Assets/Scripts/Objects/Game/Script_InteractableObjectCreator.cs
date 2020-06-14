using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectCreator : MonoBehaviour
{
    public Script_InteractableObject InteractableObjectPrefab;
    public Script_Switch SwitchPrefab;
    public Script_LightSwitch LightSwitchPrefab;
    public Script_InteractableObjectText InteractableObjectTextPrefab;
    public Script_PushablesCreator pushablesCreator;
    
    private Light[] lights;
    private Sprite OnSprite;
    private Sprite OffSprite;


    public float defaultOnIntensity;
    public float defaultOffIntensity;

    public void SetupInteractableObjectsText(
        Transform textObjectParent,
        List<Script_InteractableObject> interactableObjects,
        Vector3 rotationAdjustment,
        Script_DialogueManager dialogueManager,
        Script_Player player,
        Vector3 worldOffset,
        bool isInitialize
    )
    {
        for (int i = 0; i < textObjectParent.childCount; i++)
        {
            Script_InteractableObjectText iObj = textObjectParent.GetChild(i)
                .GetComponent<Script_InteractableObjectText>();
            interactableObjects.Add(iObj);
            
            if (isInitialize)
            {
                iObj.SetupDialogueNodeText(dialogueManager, player, worldOffset);
                iObj.Id = interactableObjects.Count - 1;
                
                Script_SortingOrder so = iObj.GetRendererChild().GetComponent<Script_SortingOrder>();
                iObj.Setup(so.enabled, so.sortingOrderIsAxisZ, so.offset);
            }
        }

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("interactable objects count: " + interactableObjects.Count);
        }
    }

    public void SetupLightSwitches(
        Transform lightSwitchesParent,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches,
        Vector3 rotationAdjustment,
        bool[] switchesState,
        bool isInitialize
    )
    {
        for (int i = 0; i < lightSwitchesParent.childCount; i++)
        {
            Script_LightSwitch iObj = lightSwitchesParent.GetChild(i).GetComponent<Script_LightSwitch>();
            interactableObjects.Add(iObj);
            switches.Add(iObj);

            if (isInitialize)
            {
                iObj.Id = interactableObjects.Count - 1;
                iObj.switchId = switches.Count - 1;
                iObj.SetupSceneLights(iObj.isOn);
                
                // TODO: REMOVE
                Script_SortingOrder so = iObj.GetRendererChild().GetComponent<Script_SortingOrder>();
                iObj.Setup(so.enabled, so.sortingOrderIsAxisZ, so.offset);
            }
        }

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("Switches Count: " + switches.Count);
            Debug.Log("IO Count: " + interactableObjects.Count);
        }
    }

    public void CreateInteractableObjects(
        Model_InteractableObject[] interactableObjectsData,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches,
        Vector3 rotationAdjustment,
        Script_DialogueManager dialogueManager,
        Script_Player player,
        bool[] switchesState,
        bool isForceSortingLayer,
        bool isSortingLayerAxisZ,
        int offset
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
                iObj.nameId = interactableObjectsData[i].nameId;
                iObj.Setup(isForceSortingLayer, isSortingLayerAxisZ, offset);
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
                iObj.nameId = interactableObjectsData[i].nameId;
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
                iObj.Setup(isForceSortingLayer, isSortingLayerAxisZ, offset);
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
                iObj.nameId = interactableObjectsData[i].nameId;
                iObj.switchId = switches.Count - 1;
                iObj.SetupSwitch(
                    switchesState == null        
                        ? interactableObjectsData[i].isOn
                        : switchesState[switches.Count - 1],
                    OnSprite,
                    OffSprite
                );
                iObj.Setup(isForceSortingLayer, isSortingLayerAxisZ, offset);
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
                iObj.nameId = interactableObjectsData[i].nameId;
                iObj.Setup(isForceSortingLayer, isSortingLayerAxisZ, offset);
            }
        }
    }

    public void SetupPushables(
        Transform pushablesParent,
        List<Script_InteractableObject> interactableObjects,
        List<Script_Pushable> pushables,
        bool isInit
    )
    {
        pushablesCreator.SetupPushables(
            pushablesParent, interactableObjects, pushables, isInit
        );
    }

    public void DestroyInteractableObjects(
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches,
        List<Script_Pushable> pushables
    )
    {
        foreach(Script_InteractableObject io in interactableObjects)
        {
            if (io)    Destroy(io.gameObject);
        }    

        interactableObjects.Clear();
        switches.Clear();
        pushables.Clear();
    }

    public void ClearInteractableObjects(
        List<Script_InteractableObject> interactableObjects,
        List<Script_Switch> switches,
        List<Script_Pushable> pushables
    )
    {
        interactableObjects.Clear();
        switches.Clear();
        pushables.Clear();
    }
}
