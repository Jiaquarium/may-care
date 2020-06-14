using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_CharacterStats))]
public class Script_Character : Script_Interactable
{
    public void Setup()
    {
        GetComponent<Script_CharacterStats>().Setup();
    }    
}
