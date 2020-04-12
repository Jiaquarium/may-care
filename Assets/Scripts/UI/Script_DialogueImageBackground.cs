using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_DialogueImageBackground : MonoBehaviour
{
    public Text extraText;
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
