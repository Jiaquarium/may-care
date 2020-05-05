using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonHandler : MonoBehaviour
{
    public bool HandleAction(
        List<Script_Demon> demons,
        Script_Player player,
        Vector3 desiredLocation,
        string action
    )
    {
        for (int i = 0; i < demons.Count; i++)
        {
            if (
                desiredLocation.x == demons[i].transform.position.x
                && desiredLocation.z == demons[i].transform.position.z
            )
            {
                if (action == Script_KeyCodes.Action2)
                {
                    player.EatDemon();
                    demons[i].DefaultAction();
                }

                return true;
            }
        }

        return false;
    }

    public void EatDemon(int Id, List<Script_Demon> demons)
    {
        for (int i = 0; i < demons.Count; i++)
        {
            if (demons[i].Id == Id)
            {
                /*
                    NOTE: the demon gameObject will wait for animation to actually
                    be destroyed
                */
                demons.RemoveAt(i);
            }
        }
    }
}
