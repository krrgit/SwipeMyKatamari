using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : Singleton<SwipeDetection>
{
    [SerializeField] private float minDist = 0.2f;
    [SerializeField] private float maxDuration = 1f;
    [SerializeField] private float swipeThreshold = 0.9f;
    private TouchManager touchManager;
    private Camera cameraMain;
    private Vector3 startPos;
    private float startTime;
    
    public delegate void RightSwipeEvent(float speed);
    public event RightSwipeEvent OnRightSwipe;
    public delegate void LeftSwipeEvent(float speed);
    public event LeftSwipeEvent OnLeftSwipe;
    
    // Start is called before the first frame update
    void Awake()
    {
        touchManager = TouchManager.Instance;
        cameraMain = Camera.main;
    }

    void OnEnable()
    {
        touchManager.OnStartTouch += SwipeStart;
        touchManager.OnEndTouch += SwipeEnd;
    }
    
    void OnDisable()
    {
        touchManager.OnStartTouch -= SwipeStart;
        touchManager.OnEndTouch -= SwipeEnd;
    }

    void SwipeStart(Vector3 position, float time)
    {
        startPos = position;
        startTime = time;
    }
    
    void SwipeEnd(Vector3 position, float time)
    {
        DetectSwipe(position, time);
    }

    void DetectSwipe(Vector3 endPos, float endTime)
    {
        float dist = Vector3.Distance(startPos, endPos);
        float duration = endTime - startTime;
        
        // print("swipe | dist: " + dist + " | dur: " + duration);
         
        if (dist >= minDist && duration <= maxDuration)
        {
            Vector3 direction = (endPos - startPos);
            direction.Normalize();
            SwipeDirection(direction, endTime);
        }
    }

    void SwipeDirection(Vector3 direction,float endTime)
    {
        Vector3 right = Vector3.Project(direction, cameraMain.transform.right);
        Vector3 up = Vector3.Project(direction, cameraMain.transform.up);
        float absX = Mathf.Abs(right.magnitude);
        float absY = Mathf.Abs(up.magnitude);
        
        // print("swipe | mag: " + direction.magnitude + " | dur: " + (endTime-startTime));
        
        if (absX < swipeThreshold
            && absY < swipeThreshold) return;

        if (absX > absY)
        {
            float speed = right.magnitude / (endTime - startTime);
            if ( Vector3.Angle(right,cameraMain.transform.right) < 90)
            {
                OnRightSwipe?.Invoke(speed);
                // print("Swipe Right. Spd: " + speed);
            }
            else
            {
                OnLeftSwipe?.Invoke(speed);
                // print("Swipe Left. Spd: " + speed);
            }
        }
        else
        {
            float speed = right.magnitude / (endTime - startTime);
            
            if ( Vector3.Angle(up,cameraMain.transform.up) < 90)
            {
                print("Swipe Up");
            }
            else
            {
                print("Swipe Down");
            }
        }
    }
}
