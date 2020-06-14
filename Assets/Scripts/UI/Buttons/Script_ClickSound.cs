using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Script_ClickSound : MonoBehaviour, ISubmitHandler
{
    public Script_InventoryAudioSettings settings;
    [SerializeField]
    protected AudioSource source;
    

    protected virtual void Awake()
    {
        source = settings.clickAudioSource;
    }

    public void OnSubmit(BaseEventData e)
    {
        source.PlayOneShot(settings.clickSFX, settings.clickVolume);
    }
}
