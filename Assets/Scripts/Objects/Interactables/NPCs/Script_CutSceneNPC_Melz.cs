using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_CutSceneNPC_Melz : Script_CutSceneNPC
{
    public AudioClip fadeOutSFX;


    [SerializeField] private GameObject glimmer;
    private Coroutine fadeOutCo;
    private Script_AudioOneShotSource audioOneShotSource;

    public override void FadeOut(Action cb)
    {        
        audioOneShotSource = game.CreateAudioOneShotSource(transform.position);
        audioOneShotSource.Setup(fadeOutSFX);
        audioOneShotSource.PlayOneShot();
        
        fadeOutCo = StartCoroutine(
            rendererChild.GetComponent<Script_SpriteFadeOut>().FadeOutCo(cb)
        );
    }

    public override void Glimmer()
    {
        glimmer.GetComponent<Script_Glimmer>().Glimmer();
    }
}
