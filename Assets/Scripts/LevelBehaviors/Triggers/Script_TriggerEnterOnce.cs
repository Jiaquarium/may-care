using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_TriggerEnterOnce : MonoBehaviour
{
    public Script_Game game;
    public string Id;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Const_Tags.Player)
        {
            game.ActivateTrigger(Id);
            this.gameObject.SetActive(false);
        }
    }
}
