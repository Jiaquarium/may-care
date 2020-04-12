using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectText : Script_InteractableObject
{
    private Model_Dialogue dialogue;
    private Script_DialogueManager dialogueManager;
    private Script_Player player;

    public override void SetupText(
        Script_DialogueManager _dialogueManager,
        Script_Player _player,
        Model_Dialogue _dialogue
    )
    {
        dialogueManager = _dialogueManager;
        dialogue = _dialogue;
        player = _player;

        if (Debug.isDebugBuild)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public override void ActionDefault()
    {
        if (!player.GetIsTalking())     dialogueManager.StartDialogue(dialogue, "read");
        else                            dialogueManager.DisplayNextDialoguePortion();
    }

    public override void ActionB()
    {
        print("calling ActionB");
        if (player.GetIsTalking())      dialogueManager.SkipTypingSentence();
    }
}
