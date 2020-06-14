using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script_HintManager : MonoBehaviour
{
    public CanvasGroup hintCanvas;
    public TextMeshProUGUI hintCanvasText;
    [SerializeField]
    private Script_Game game;

    
    public void ShowHint(string s)
    {
        hintCanvasText.text = Script_Utils.FormatString(s);
        hintCanvas.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        hintCanvas.gameObject.SetActive(false);
        hintCanvasText.text = "";
    }

    public void Setup()
    {
        HideHint();
        game = transform.parent.GetComponent<Script_Game>();
    }
}
