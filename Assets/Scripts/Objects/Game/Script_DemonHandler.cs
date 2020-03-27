using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonHandler : MonoBehaviour
{
    public bool HandleAction(
        List<Script_Demon> demons,
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
                if (action == "Action2")
                {
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
                Destroy(demons[i].gameObject);
                demons.RemoveAt(i);
            }
        }
    }
}
