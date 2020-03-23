using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Script_Dialogue
{
    public string name;
    // (min amount of lines to use, max)
    [TextArea(3, 10)]
    public string[] sentences;
}
