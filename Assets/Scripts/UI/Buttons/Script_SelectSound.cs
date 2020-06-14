using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Script_SelectSound : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public Script_InventoryAudioSettings settings;
    public Script_EventSystemLastSelected eventSystem;
    public GameObject transition;
    [SerializeField]
    private Button button { get { return GetComponent<Button>(); } }
    [SerializeField]
    private AudioSource source;
    

    void Awake()
    {
        source = settings.selectAudioSource;
    }

    public void OnSubmit(BaseEventData e)
    {
    }

    public void OnSelect(BaseEventData e)
    {
        // other option is to tell manager which sounds to play
        // lateUpdate can decide which one to pick (onSubmit takes priority)
        if (
            eventSystem.lastSelected == transition
            || eventSystem.lastSelected == null 
        ) 
        {
            return;
        }
        PlaySFX();
    }


    void PlaySFX()
    {
        source.PlayOneShot(settings.selectSFX, settings.selectVolume);
    }
}
