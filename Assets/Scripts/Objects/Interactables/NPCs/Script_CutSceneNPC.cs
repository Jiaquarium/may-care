using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    cut scene NPCs just show up for cut scene and disappear afterwards
*/
public class Script_CutSceneNPC : Script_StaticNPC
{
    public int CutSceneNPCId;

    public override void Freeze(bool isFrozen)
    {
        rendererChild.GetComponent<Animator>().SetBool("isFrozen", isFrozen);
    }
    
    public override void Setup
    (
        Model_Dialogue dialogue,
        Script_DialogueNode[] dialogueNodes,
        Model_MoveSet[] movesets
    )
    {
        base.Setup(dialogue, dialogueNodes, movesets);
    }
}
