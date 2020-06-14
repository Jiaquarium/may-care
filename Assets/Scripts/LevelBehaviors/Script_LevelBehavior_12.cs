using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_12 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isDone;
    public bool[] pushablesStates;
    /* ======================================================================= */
    
    public bool isInit = true;
    public Transform playerReflectionEro;
    public Transform pushablesParent;
    public Transform triggers;

    public override void ActivateObjectTrigger(string Id, Collider col){
        if (Id == "fireplace" && !isDone)
        {
            col.transform.parent.gameObject.SetActive(false);
            print("success! you got 1 book into the fireplace");
            CheckPushablesStates();
        }
    }

    void CheckPushablesStates()
    {    
        foreach (Transform t in triggers)
        {
            print(t.GetComponent<Script_PushableTriggerEnter>().count);
            if (t.GetComponent<Script_PushableTriggerEnter>().count > 0)    return;        
        }

        OnPuzzleComplete();
    }

    void OnPuzzleComplete()
    {
        print("PUZZLE COMPLETE!!!");
        isDone = true;
        game.DisableExits(false, 0);
    }

    public override void Setup()
    {
        game.SetupPlayerReflection(playerReflectionEro);
        game.SetupPushables(pushablesParent, isInit);
        isInit = false;

        if (!isDone)
        {
            game.DisableExits(true, 0);
        }
        else
        {
            game.DisableExits(false, 0);
        }
    }
}
