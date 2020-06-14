using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CutSceneNPCCreator : MonoBehaviour
{
    public Script_CutSceneNPC CutSceneNPCPrefab;
    public Script_CutSceneNPC_Melz CutSceneNPCMelzPrefab;
    
    public void SetupCutSceneNPC(
        Script_CutSceneNPC cutSceneNPC,
        List<Script_StaticNPC> NPCs,
        List<Script_CutSceneNPC> cutSceneNPCs
    )
    {
        // TODO: unneeded parameters only to satisfy instantiation way
        cutSceneNPC.Setup(
            cutSceneNPC.dialogue,
            cutSceneNPC.dialogueNodes,
            new Model_MoveSet[0]
        );
        
        NPCs.Add(cutSceneNPC);
        cutSceneNPCs.Add(cutSceneNPC);

        cutSceneNPC.StaticNPCId = NPCs.Count - 1;
        cutSceneNPC.CutSceneNPCId = cutSceneNPCs.Count - 1;
    }
    
    public void CreateCutSceneNPC(
        Model_NPC NPCsData,
        List<Script_StaticNPC> NPCs,
        List<Script_CutSceneNPC> cutSceneNPCs,
        int staticNPCId
    )
    {
        Script_CutSceneNPC prefab = CutSceneNPCPrefab;

        if (NPCsData.type == "melz")
        {
            prefab = CutSceneNPCMelzPrefab;
        }

        Script_CutSceneNPC cutSceneNPC = Instantiate(
            prefab,
            NPCsData.NPCSpawnLocation,
            Quaternion.identity
        );

        NPCs.Add(cutSceneNPC);
        cutSceneNPCs.Add(cutSceneNPC);

        cutSceneNPC.StaticNPCId = staticNPCId;
        cutSceneNPC.CutSceneNPCId = cutSceneNPCs.Count - 1;
        cutSceneNPC.Setup(
            NPCsData.dialogue,
            NPCsData.dialogueNodes,
            new Model_MoveSet[0]
        );

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("cutSceneNPCs Count: " + cutSceneNPCs.Count);
        }
    }
}
