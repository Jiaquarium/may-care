using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_DialogueNode
{
    public Model_Dialogue dialogue;
    public bool isUnskippable;
    public Model_DialogueNode[] children;
    public string choiceText;
}
