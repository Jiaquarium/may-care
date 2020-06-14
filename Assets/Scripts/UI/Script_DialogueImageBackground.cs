using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script_DialogueImageBackground : MonoBehaviour
{
    public TextMeshProUGUI extraText;
    // Update is called once per frame
    void Update()
    {
        if (extraText.text != "")
        {
            GetComponent<Image>().enabled = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
