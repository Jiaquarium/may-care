using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SavePoint : Script_Interactable
{
    public Script_Game game;
    public SpriteRenderer spriteRenderer;
    public Script_DialogueManager dm;
    public Script_DialogueNode dialogueNode;
    public Model_SavePointData data;

    public void HandleAction(string action)
    {
        if (action == Const_KeyCodes.Action1)
        {
            SaveDialogue();
        }
        else if (action == Const_KeyCodes.Skip)
        {
            SkipDialogue();
        }
    }

    public Model_SavePointData GetData()
    {
        return data;
    }

    void SaveDialogue()
    {
        if (!game.GetPlayerIsTalking()) dm.StartDialogueNode(dialogueNode);
        else dm.DisplayNextDialoguePortion();
    }

    void SkipDialogue()
    {
        if (game.GetPlayerIsTalking())  dm.SkipTypingSentence();   
    }

    void AdjustRotation()
    {
        spriteRenderer.transform.forward = Camera.main.transform.forward;
    }

    public void Setup()
    {
        AdjustRotation();
    }
}
