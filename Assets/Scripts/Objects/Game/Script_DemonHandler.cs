using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonHandler : MonoBehaviour
{
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
