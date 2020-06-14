using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CutSceneManager : MonoBehaviour
{
    public Script_Game g;
    public float melanholicTitleFadeInTime;
    public Script_CanvasGroupController parentCutSceneCanvasGroup;
    public Script_CanvasGroupFadeInOut melanholicTitleCutScene;
    private Script_CanvasGroupController_CutScene melanholicTitleCutSceneController;

    public void MelanholicTitleCutScene()
    {
        melanholicTitleCutSceneController.SetActiveForFade();

        StartCoroutine(
            melanholicTitleCutScene
                .FadeInCo(melanholicTitleFadeInTime, null)
        );

        // jammin latin
        g.SwitchBgMusic(5);
    }

    public void Setup()
    {
        parentCutSceneCanvasGroup.Open();
        melanholicTitleCutSceneController = melanholicTitleCutScene
            .GetComponent<Script_CanvasGroupController_CutScene>();
        melanholicTitleCutSceneController.Setup();
    }
}
