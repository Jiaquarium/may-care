using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_13_BioArt : Script_LevelBehavior
{
    public bool isComplete;
    public Script_Exits exitsHandler;
    public Script_DialogueManager dm;
    public Model_Locations triggerLocs;
    public Script_DialogueNode StartNode;


    private bool beginDialogue;
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
            game.ChangeStateInteract();
        }
    }

    protected override void HandleAction()
    {
        base.HandleDialogueAction();
    }
    
    public override void Setup()
    {
        isFinishDialogue = false;
        beginDialogue = false;
        isTriggerActivated = false;
        game.CreateNPCs(); 
    }
}
