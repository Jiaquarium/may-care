using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PushableTriggerEnter : MonoBehaviour
{
    public int count;
    [SerializeField] private Script_Game game;
    [SerializeField] private string Id;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Const_Tags.Pushable)
        {
            count--;
            if (count == 0)     this.gameObject.SetActive(false);
            game.ActivateObjectTrigger(Id, other);
        }
    }

    void OnTriggerStay(Collider other) {
        print(other.tag);    
    }
}
