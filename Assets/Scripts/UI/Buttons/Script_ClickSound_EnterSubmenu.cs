using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Script_ClickSound_EnterSubmenu : MonoBehaviour, ISubmitHandler
{
    public Script_InventoryAudioSettings settings;
    [SerializeField]
    protected AudioSource source;
    

    void Awake()
    {
        source = settings.clickEnterSubmenuAudioSource;
    }

    public void OnSubmit(BaseEventData e)
    {
        source.PlayOneShot(
            settings.clickEnterSubmenuSFX,
            settings.clickEnterSubemenuSFXVolume
        );
    }
}
