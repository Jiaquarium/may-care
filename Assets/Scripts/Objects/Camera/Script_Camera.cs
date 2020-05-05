using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Camera : MonoBehaviour
{
    public Transform target;
    
    public float defaultOrthographicSize;
    public Vector3 offset;
    public float defaultTrackingSpeed;
    public float moveToTargetSpeed;
    public float moveToTargetInstantlySpeed;
    public float timerMax;
    public float progress;
    public Vector3 endPosition;
    public Vector3 startPosition;
    public float cameraTrackedPlayerDistance;
    public Vector3 rotationAdjToFaceCamera;


    private Vector3 OffsetDefault;
    public float speed;
    private float timer;
    private bool isTrackPlayer = true;
    private bool shouldMoveToTarget = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        OffsetDefault = offset;
        speed = defaultTrackingSpeed;
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

        if (shouldMoveToTarget)
        {
            CheckMovedToTarget();
        }
        
        Move();
    }

    void Move()
    {
        if (progress >= 1f) return;
        progress += speed;
        transform.position = Vector3.Lerp(startPosition, endPosition, progress);
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


    public void MoveToTarget()
    {
        shouldMoveToTarget = true;
    }

    void CheckMovedToTarget()
    {
        FastTrackSpeed();
        
        float cameraDistanceFromEndingPos = Vector3.Distance(transform.position, endPosition);

        if (cameraDistanceFromEndingPos <= cameraTrackedPlayerDistance)
        {
            speed = defaultTrackingSpeed;
            shouldMoveToTarget = false;
        }
    }

    public void SetOffsetToDefault()
    {
        offset = OffsetDefault;
    }

    public void SetOffset(Vector3 _offset)
    {
        offset = _offset;
    }

    public void SetUntrackPlayer()
    {
        isTrackPlayer = false;
    }

    public void SetTarget<T>(T gameObject) where T : Transform
    {
        target = gameObject;
    }

    public void SetTrackPlayer()
    {
        isTrackPlayer = true;
    }

    public void FastTrackSpeed()
    {
        speed = moveToTargetSpeed;
    }

    public void InstantTrackSpeed()
    {
        speed = moveToTargetInstantlySpeed;
    }

    public void DefaultSpeed()
    {
        speed = defaultTrackingSpeed;
    }

    public Vector3 GetRotationAdjustment()
    {
        return rotationAdjToFaceCamera;
    }

    public void SetOrthographicSizeDefault()
    {
        GetComponent<Camera>().orthographicSize = defaultOrthographicSize;
    }

    public void SetOrthographicSize(float size)
    {
        GetComponent<Camera>().orthographicSize = size;
    }
}
