using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 2;

    [SerializeField] private float currSpeed;

    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float holdRotMult = 1;
    [SerializeField] private float swipeRotSpeedThreshold = 5;
    
    private TouchManager touchManager;

    private Vector3 startRot;
    private float startTime;
    private Vector3 startTouchPos;

    private float rotDelta;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        touchManager.OnStartTouch += StopSpin;
        touchManager.OnStartTouch += StoreRotation;
        touchManager.OnSwipeHoldTouch += HoldRotation;
        touchManager.OnEndTouch += TrySpin;
    }

    private void OnDisable()
    {
        touchManager.OnStartTouch -= StopSpin;
        touchManager.OnStartTouch -= StoreRotation;
        touchManager.OnSwipeHoldTouch -= HoldRotation;
        touchManager.OnEndTouch -= TrySpin;
    }
    void StopSpin(Vector3 rawPosition, float time)
    {
        currSpeed = 0;
    }

    void TrySpin(Vector3 touchPosition, float time)
    {
        float duration = time - startTime;
        float rotDist = Mathf.Abs(transform.rotation.eulerAngles.y - startRot.y);
        
        // print("swipe | dur: " + duration + " | rotDist: " + rotDist);
        
        if (rotDelta >= swipeRotSpeedThreshold)
        {
            currSpeed = maxSpeed * Mathf.Sign(touchPosition.x - startTouchPos.x);
        }
        else
        {
            currSpeed = 0;
        }
    }

    void StoreRotation(Vector3 touchPosition, float time)
    {
        startRot = transform.rotation.eulerAngles;
        startTime = time;
        startTouchPos = touchPosition;
    }

    void HoldRotation(Vector3 holdDirection)
    {
        Vector3 forward = transform.forward;
        transform.rotation = Quaternion.Euler(startRot.x, startRot.y + (holdDirection.x*holdRotMult), startRot.z);
        float angle = Vector3.Angle(forward, transform.forward);

        if (rotDelta != angle)
        {
            rotDelta = angle > 0 ? angle: rotDelta; 
            // print("rotDelta: " + rotDelta);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        int inputX = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);

        transform.rotation *= Quaternion.Euler(0, currSpeed, 0);

        currSpeed = Mathf.Abs(currSpeed) > 0 ? currSpeed - ((Mathf.Sign(currSpeed) * friction) * Time.deltaTime) : 0;
    }
}
