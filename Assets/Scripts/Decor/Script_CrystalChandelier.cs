using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CrystalChandelier : MonoBehaviour
{
    [SerializeField]
    private Animator a; 

    void Awake() {
        a = GetComponent<Animator>();
    }

    public void StartSpinning()
    {
        a.SetBool("isSpinning", true);
    }

    public void StopSpinning()
    {
        a.SetBool("isSpinning", false);
    }
}
