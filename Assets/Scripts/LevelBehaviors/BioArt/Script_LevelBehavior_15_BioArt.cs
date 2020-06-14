using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_15_BioArt : Script_LevelBehavior
{
    public bool isComplete;
    public GameObject target;
    public Script_DialogueNode StartNode;
    public Script_DialogueManager dm;
    public Model_Locations triggerLocs;

    private bool isTriggerActivated;
    private bool isFinishDialogue;
    
    protected override void HandleTriggerLocations()
    {
        foreach (Vector3 loc in triggerLocs.locations)
        {
            if (
                game.GetPlayerLocation() == loc
                && game.state == "interact"
                && !isTriggerActivated
            )
            {
                isTriggerActivated = true;
                game.ChangeStateCutScene();
                game.ChangeCameraTargetToGameObject(target);

                dm.StartDialogueNode(StartNode);
            }
        }

        if (
            game.state == "cut-scene"
            && !game.GetPlayerIsTalking()
            && !isFinishDialogue
        )
        {
            isFinishDialogue = true;
            isComplete = true;
            game.CameraTargetToPlayer();
            game.ChangeStateInteract();
        }
    }

    protected override void HandleAction()
    {
        base.HandleDialogueAction();
    }
    
    public override void Setup()
    {
        game.CreateDemons(new bool[1]{true});
    }    
}
