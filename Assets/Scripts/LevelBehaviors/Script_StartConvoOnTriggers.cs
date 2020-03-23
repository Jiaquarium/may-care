using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_StartConvoOnTriggers : MonoBehaviour
{
    public Script_Game game;
    public Script_LocationsModel[] triggerLocations;
    public Script_Dialogue[] dialogues;
    private int activeTriggerIndex = 0;
    private bool isDone = false;

    // Start is called before the first frame update
    void Start()
    {
                    
    }
 
    // Update is called once per frame
    void Update()
    {
        HandleAction();
        
        if (isDone) return;

        foreach (Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
            )
            {
                game.ChangeStateCutScene();
                
                // game.PlayerFaceDirection("down");
                game.StartDialogue(dialogues[activeTriggerIndex]);
                
                /*
                    trigger locations must be 1 < dialogue
                */
                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                else activeTriggerIndex++;
            }
        }
    }

    void HandleAction()
    {
        if (
            game.state == "cut-scene"
            && game.state != "cut-scene_npc-moving"
            && !game.GetPlayerIsTalking()
        )
        {
            // trigger ActuallyMove() in MovingNPC to exit
            if (isDone)
            {
                game.ChangeStateCutSceneNPCMoving();
            }
            else
            {
                game.ChangeStateCutSceneNPCMoving();
            }
        }

        if (Input.GetButtonDown("Action1") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Action1");
        }

        if (Input.GetButtonDown("Submit") && game.state == "cut-scene")
        {
            game.HandleContinuingDialogueActions("Submit");
        }
    }
}
