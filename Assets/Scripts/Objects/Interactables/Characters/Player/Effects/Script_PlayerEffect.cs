using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerEffect : MonoBehaviour
{
    [SerializeField] private GameObject questionMark;

    public void QuestionMark(bool isShow)
    {
        if (isShow)
        {
            questionMark
                .GetComponent<Script_PlayerEffectAnimate>()
                .QuestionMark();
        }
        else
        {
            questionMark
                .GetComponent<Script_PlayerEffectAnimate>()
                .HideQuestionMark();
        }
    }

    public void Setup()
    {
        questionMark
            .GetComponent<Script_PlayerEffectAnimate>()
            .Setup();
    }
}
