using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_DialogueNode
{
    public Model_Dialogue dialogue;
    public bool isUnskippable;
    public Script_DialogueNode[] children;
    public string choiceText;
    public string action;
}
