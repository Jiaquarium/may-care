using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_9 : Script_LevelBehavior
{
    public bool isInitialized;
    public Script_ProximitySpeaker speakerPrefab;
    public Vector3 speakerLoc;
    public Script_ProximitySpeaker speaker;
    public Transform signTextParent;
    
    public override void Setup()
    {
        game.SetupInteractableObjectsText(signTextParent, !isInitialized);
        if (speaker == null)
        {
            speaker = Instantiate(
                speakerPrefab,
                speakerLoc,
                Quaternion.identity
            );
            speaker.transform.SetParent(game.bgThemeSpeakersContainer, false);
        }

        isInitialized = true;
    }
}
