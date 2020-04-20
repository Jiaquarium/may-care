using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CutSceneNPCCreator : MonoBehaviour
{
    public Script_CutSceneNPC CutSceneNPCPrefab;
    
    public void CreateCutSceneNPC(
        Model_NPC NPCsData,
        List<Script_StaticNPC> NPCs,
        List<Script_CutSceneNPC> cutSceneNPCs,
        int staticNPCId
    )
    {
        Script_CutSceneNPC cutSceneNPC = Instantiate(
            CutSceneNPCPrefab,
            NPCsData.NPCSpawnLocation,
            Quaternion.identity
        );

        NPCs.Add(cutSceneNPC);
        cutSceneNPCs.Add(cutSceneNPC);

        cutSceneNPC.StaticNPCId = staticNPCId;
        cutSceneNPC.CutSceneNPCId = cutSceneNPCs.Count - 1;
        cutSceneNPC.Setup(
            NPCsData.sprite,
            NPCsData.dialogue,
            new Model_MoveSet[0]
        );
    }
}
