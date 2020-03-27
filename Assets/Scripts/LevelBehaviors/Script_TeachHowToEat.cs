using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_TeachHowToEat : MonoBehaviour
{
    public Script_Game game;
    public Model_Locations[] triggerLocations;
    public Vector3[] triggerLoc;
    public Model_Dialogue dialogue;
    public Model_Dialogue[] dialogues;
    
    
    private bool isDone = false;
    private int activeTriggerIndex = 0;

    void Start()
    {
        game.DisableExits();
        
        // Ero will wait at the door as you leave this map
        game.SetMovingNPCExit(0, false);
        
        game.ChangeMovingNPCSpeed(0, 0.175f);
        
        // TODO: enable exits only when all demons are eaten       
    }

    // Update is called once per frame
    void Update()
    {
        HandleAction();
        
        foreach(Vector3 loc in triggerLocations[activeTriggerIndex].locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && !isDone
            )
            {
                game.PauseBgMusic();
                game.ChangeStateCutScene();
                
                if (activeTriggerIndex == 0)    game.PlayerFaceDirection("down");
                if (activeTriggerIndex == 1)
                {
                    game.PlayerFaceDirection("right");
                    game.NPCFaceDirection(0, "left");
                }
                game.ChangeCameraTargetToNPC(0);
                game.StartDialogue(dialogues[activeTriggerIndex]);

                if (activeTriggerIndex == triggerLocations.Length - 1) isDone = true;
                else activeTriggerIndex++;
            }
        }
    }

    void HandleAction()
    {
        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
        )
        {
            if (!isDone)
            {
                game.ChangeStateCutSceneNPCMoving();
                game.TriggerMovingNPCMove(0);
            } else
            {
                game.EnableExits();
                game.ChangeStateInteract();
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
