using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private Vector3 OffsetDefault;
    public float speed;
    private float timer;
    public float timerMax;
    public float progress;
    public Vector3 endPosition;
    public Vector3 startPosition;
    private bool isTrackPlayer = true;
    
    // Start is called before the first frame update
    void Start()
    {
        OffsetDefault = offset;
        progress = 0f;
        timer = timerMax;
        startPosition = transform.position;
        endPosition = transform.position;     
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null || !isTrackPlayer) return;

        Timer();
        Move();
    }

    void Timer()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            timer = timerMax;
            UpdateEndPosition();
        }
    }

    void UpdateEndPosition()
    {
        endPosition = target.position + offset;
        startPosition = transform.position;
        progress = 0f;
    }

    void Move()
    {
        if (progress >= 1f) return;
        progress += speed;
        transform.position = Vector3.Lerp(startPosition, endPosition, progress);
    }

    public void SetOffsetToDefault()
    {
        offset = OffsetDefault;
    }

    public void SetUntrackPlayer()
    {
        isTrackPlayer = false;
    }

    public void SetTrackPlayer()
    {
        isTrackPlayer = true;
    }
}
