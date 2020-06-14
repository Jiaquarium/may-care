using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonCreator : MonoBehaviour
{
    public Script_Demon DemonPrefab;
    public Script_Demon DemonDorusPrefab;
    public Script_Demon DemonSedgewickPrefab;

    public void CreateDemons(
        bool[] spawnState,
        Model_Demon[] demonsData,
        List<Script_Demon> demons
    )
    {
        Script_Demon demon;

        if (demonsData.Length == 0)    return;

        for (int i = 0; i < demonsData.Length; i++)
        {
            if (spawnState != null && spawnState[i] == false) continue;

            demon = Instantiate(
                demonsData[i].prefab,
                demonsData[i].demonSpawnLocation,
                Quaternion.identity
            );

            demons.Add(demon);
            demon.Id = i;
            demon.Setup(
                demonsData[i].thought,
                demonsData[i].deathCry
            );
        }

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("Demons Count: " + demons.Count);
        }
    }

    public void SetupDemons(
        Transform demonsParent,
        bool[] spawnState,
        List<Script_Demon> demons
    )
    {
        Script_Demon[] demonsInScene = new Script_Demon[demonsParent.childCount];
        for (int i = 0; i < demonsInScene.Length; i++)
        {
            demonsInScene[i] = demonsParent.GetChild(i).GetComponent<Script_Demon>();
        }

        for (int i = 0; i < demonsInScene.Length; i++)
        {
            Script_Demon demon = demonsInScene[i];
            
            demon.Setup(demon.thought, demon.deathCrySFX);
            demons.Add(demon);
            demon.Id = i;
        }
    }
}
