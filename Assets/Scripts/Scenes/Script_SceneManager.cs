using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_SceneManager : MonoBehaviour
{
    public void SwitchSceneToTitle()
    {
        SceneManager.LoadScene(1);
    }
}