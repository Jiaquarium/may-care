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
    }
}
