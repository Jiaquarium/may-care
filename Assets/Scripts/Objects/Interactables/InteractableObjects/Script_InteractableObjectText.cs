using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_InteractableObjectText : Script_InteractableObject
{
    public Script_DialogueNode[] dialogueNodes;
    
    [SerializeField]
    private int dialogueIndex;
    private Model_Dialogue dialogue;
    private Script_DialogueManager dialogueManager;
    private Script_Player player;

    // for instantiation
    public override void SetupText(
        Script_DialogueManager _dialogueManager,
        Script_Player _player,
        Model_Dialogue _dialogue
    )
    {
        dialogueManager = _dialogueManager;
        dialogue = _dialogue;
        player = _player;
    }

    public void SetupDialogueNodeText(
        Script_DialogueManager _dialogueManager,
        Script_Player _player,
        Vector3 _worldOffset
    )
    {
        dialogueManager = _dialogueManager;
        player = _player;
        // need bc we're parented by World Transform
        worldOffset = _worldOffset;
    }

    public override void SwitchDialogueNodes(
        Script_DialogueNode[] _dialogueNodes
    )
    {
        dialogueNodes = _dialogueNodes;
    }

    public override void ActionDefault()
    {
        if (!player.GetIsTalking())
        {
            dialogueManager.StartDialogueNode(
                dialogueNodes[dialogueIndex],
                SFXOn: true,
                type: Const_DialogueTypes.Read
            );
            HandleDialogueNodeIndex();
        }
        else    dialogueManager.DisplayNextDialoguePortion();
    }

    void HandleDialogueNodeIndex()
    {
        if (dialogueIndex == dialogueNodes.Length - 1)
        {
            dialogueIndex = 0;    
        }
        else
        {
            dialogueIndex++;
        }
    }

    public override void ActionB()
    {
        if (player.GetIsTalking())      dialogueManager.SkipTypingSentence();
    }
}
