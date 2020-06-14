using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_DialogueNode
{
    public Model_Dialogue dialogue;
    public bool showDialogueOnAction;
    public bool isDynamicLines;
    public Script_DialogueNode[] children;
    public string choiceText;
    public string action;
    public string updateAction; // key to be called on update
    public string locationType; // where canvas should be
    public string type; // e.g. read,
}
