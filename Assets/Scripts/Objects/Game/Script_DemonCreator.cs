using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonCreator : MonoBehaviour
{
    public Script_Demon DemonPrefab;

    public void CreateDemons(
        Model_Demon[] demonsData,
        List<Script_Demon> demons
    )
    {
        Script_Demon demon;

        if (demonsData.Length == 0)    return;

        for (int i = 0; i < demonsData.Length; i++)
        {
            demon = Instantiate(
                DemonPrefab,
                demonsData[i].demonSpawnLocation,
                Quaternion.identity
            );

            demons.Add(demon);
            demon.Id = i;
            demon.Setup(demonsData[i].thought);
        }
    }
}
