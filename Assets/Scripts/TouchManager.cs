using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : Singleton<TouchManager>
{
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private Camera cameraMain;
    
    // Drag 
    [SerializeField] private float dradMinDist = 0.01f;
    [SerializeField] private Vector3 holdStartPosRaw;
    private Vector3 touchDir;
    private bool dragActive;
    
    // Bottom Touch
    [SerializeField] private float bottomTouchThreshold = 350;
    [SerializeField] private bool bottomTouchHold;
    
    // Tap
    [SerializeField] private float tapMinDist = 0.01f;
    [SerializeField] private float tapMinDuration = 0.3f;

    private Vector3 startTouchPosition;
    private float startTouchTime;
    
    public delegate void DragTouchEvent(Vector3 holdDirection);
    public event DragTouchEvent OnDragTouch;
    public delegate void StartTouchEvent(Vector3 worldPosition, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector3 worldPosition, float time);
    public event EndTouchEvent OnEndTouch;
    public delegate void SwipeHoldEvent(Vector3 holdDirection);
    public event SwipeHoldEvent OnSwipeHoldTouch;
    
    public delegate void BottomStartTouchEvent();
    public event BottomStartTouchEvent OnBottomStartTouch;
    public delegate void BottomEndTouchEvent();
    public event BottomEndTouchEvent OnBottomEndTouch;
    public delegate void BottomCancelTouchEvent();
    public event BottomCancelTouchEvent OnBottomCancelTouch;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        cameraMain = Camera.main;
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += PrimaryStartTouch;
        touchPressAction.canceled += PrimaryEndTouch;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= PrimaryStartTouch;
        touchPressAction.canceled -= PrimaryEndTouch;

    }

    void PrimaryStartTouch(InputAction.CallbackContext context)
    {
        Vector3 touchPos = touchPositionAction.ReadValue<Vector2>();
        touchPos.z = cameraMain.nearClipPlane;
        Vector3 worldPos = cameraMain.ScreenToWorldPoint(touchPos)-cameraMain.transform.position;

        startTouchPosition = touchPos;
        startTouchTime = Time.time;

        holdStartPosRaw = touchPositionAction.ReadValue<Vector2>();
        
        // print("raw touch pos: " + holdStartPosRaw.y);
        
        OnStartTouch?.Invoke(touchPos, Time.time);
        // OnHoldTouch?.Invoke(worldPos, Time.time);

        StartCoroutine(HoldTouch());

        if (touchPos.y <= bottomTouchThreshold)
        {
            OnBottomStartTouch?.Invoke();
            bottomTouchHold = true;
        }
    }

    void PrimaryEndTouch(InputAction.CallbackContext context)
    {
        Vector3 touchPos = touchPositionAction.ReadValue<Vector2>();
        touchPos.z = cameraMain.nearClipPlane;
        Vector3 worldPos = cameraMain.ScreenToWorldPoint(touchPos)-cameraMain.transform.position;
        OnEndTouch?.Invoke(touchPos, Time.time);

        if (bottomTouchHold)
        {
            OnBottomEndTouch?.Invoke();
            bottomTouchHold = false;
        }
        
    }

    IEnumerator HoldTouch()
    {
        while (touchPressAction.ReadValue<float>() > 0.5f)
        {
            Vector3 touchPos = touchPositionAction.ReadValue<Vector2>();

            touchDir = (touchPos - holdStartPosRaw);
            touchDir = touchDir.magnitude >= dradMinDist ? touchDir : Vector3.zero;
            
            float absX = Mathf.Abs(touchDir.x);
            float absY = Mathf.Abs(touchDir.y);
            // print("touchDir: " + touchDir.magnitude);
            
            // Swipe Up/Down first, then move freely
            // to avoid issues with camera controls
            if (!dragActive && absY > absX)
            {
                // print("drag activate. X: " + absX + " | Y: " + absY);
                dragActive = true;
            }
            else if (dragActive)
            {
                OnDragTouch?.Invoke(touchDir);
            }
            else
            {
                // print("swipe hold");
                OnSwipeHoldTouch?.Invoke(touchPos - holdStartPosRaw);
            }
            
            
            // Cancel Jump if hold tap too far
            if (bottomTouchHold)
            {
                float tapDist = Vector3.Distance(touchPos, startTouchPosition);
                if (dragActive || tapDist < tapMinDist || touchPos.y > bottomTouchThreshold)
                {
                    print("Cancel Bottom Hold");
                    OnBottomCancelTouch?.Invoke();
                    bottomTouchHold = false;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        dragActive = false;
    }

}
