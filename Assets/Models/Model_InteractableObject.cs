using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_InteractableObject
{
    public Vector3 objectSpawnLocation;
    public Sprite sprite;
    public string type;
    public Light[] lights;
    public float lightOnIntensity;
    public float lightOffIntensity;
}
