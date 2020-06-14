using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_11_BioArt : Script_LevelBehavior
{
    public Script_DialogueManager dm;
    public Script_DialogueNode finalNode;
    public Script_Exits exitsHandler;
    public Script_LevelBehavior_12_BioArt lb12;
    public Script_LevelBehavior_13_BioArt lb13;
    public Script_LevelBehavior_14_BioArt lb14;
    public Script_LevelBehavior_15_BioArt lb15;
    public Script_LevelBehavior_16_BioArt lb16;

    public float exitWaitTime;

    private bool isDone;
    private bool isActivated;
    private bool isFinalDialogueDone;
    
    protected override void HandleOnEntrance()
    {
        if (!exitsHandler.isFadeIn && !isActivated)
        {
            HandleLastConvo();
        }   
    }

    void HandleLastConvo()
    {
        if (
            lb12.isComplete
            && lb13.isComplete
            && lb14.isComplete
            && lb15.isComplete
            && lb16.isComplete
            && !isDone
        )
        {
            isDone = true;
            game.ChangeStateCutScene();
            dm.StartDialogueNode(finalNode);
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "exit")
            && !isFinalDialogueDone
        )
        {
            Application.Quit();
        }
    }

    public override void Setup()
    {
        isActivated = false;
        isDone = false;
        isFinalDialogueDone = false;
    }    
}
