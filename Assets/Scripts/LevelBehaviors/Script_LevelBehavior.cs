using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior : MonoBehaviour
{
    public Script_Game game;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTriggerLocations();
        HandleAction();
    }

    protected virtual void OnDisable() {}
    protected virtual void HandleTriggerLocations() {}
    protected virtual void HandleAction() {}

    public virtual void Setup()
    {
        print("setting up levelBehavior");
    }
}
