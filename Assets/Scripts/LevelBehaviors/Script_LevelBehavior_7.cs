using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_7 : Script_LevelBehavior
{
    public bool isDone = false;
    public bool isActivated = false;
    public Script_DialogueManager dm;
    public Model_DialogueNode node;
    
    protected override void HandleOnEntrance()
    {
        if (game.state == "interact" && !isActivated)
        {
            print("PLAYER CAN INTERACT");
            isActivated = true;

            dm.StartDialogueNode(node);
            game.ChangeStateCutScene();
        }
        
        // detect when dialogue is over
        if (game.state == "cut-scene" && !game.GetPlayerIsTalking())
        {
            print("DIALOGUE IS DONE");
            game.ChangeStateInteract();
        }
    }    

    protected override void HandleAction()
    {
        base.HandleDialogueAction();
    }

    public override void Setup()
    {
        if (!isDone)
        {
            game.CreateNPCs();
        }
    }
}
