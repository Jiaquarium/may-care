using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    meant to be instantiated through code
*/
public class Script_ProximitySpeaker_Persist : Script_ProximitySpeaker
{
    public int levelToPersistUntil;
    [SerializeField]
    private int originLevel;
    
    
    protected override void OnDisable()
    {
        // to null base     
    }

    protected override void Awake() {
        base.Awake();
        
        originLevel = game.level;
    }

    protected override void Update()
    {
        if (game.level == originLevel)
        {
            audioSource.volume = 1f;
            AdjustVolume();
        }
        else if (game.level < originLevel || game.level > levelToPersistUntil)
        {
            audioSource.Stop();
            Destroy(this.gameObject);
        }
    }
}
