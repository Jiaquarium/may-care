using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_InteractableObject
{
    public Vector3 objectSpawnLocation;
    public Sprite onSprite;
    public Sprite offSprite;
    public string type;
    public Light[] lights;
    public float lightOnIntensity;
    public float lightOffIntensity;
    public Model_Dialogue dialogue;
    public bool isOn;
}
