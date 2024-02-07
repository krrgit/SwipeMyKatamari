using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float movePower = 4;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxAngularVelocity = 25f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minJump = 5;
    [SerializeField] private float maxJump = 15;
    [SerializeField] private float jumpChargeMaxTime = 5;
    
    [SerializeField] private float lerpSpeed = 10;

    [SerializeField] private Vector3 startPos;
    private Vector2 touchDir;

    private Vector3 moveDir;
    private bool moveActive;
    
    private Camera cameraMain;
    private TouchManager touchManager;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
        cameraMain = Camera.main;
    }

    private void OnEnable()
    {
        touchManager.OnStartTouch += StartMove;
        touchManager.OnDragTouch += MoveInput;
        touchManager.OnEndTouch += StopMove;
        touchManager.OnBottomTouch += Jump;
    }
    private void OnDisable()
    {
        touchManager.OnStartTouch -= StartMove;
        touchManager.OnDragTouch -= MoveInput;
        touchManager.OnEndTouch -= StopMove;
        touchManager.OnBottomTouch -= Jump;
    }

    void Start()
    {
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void StartMove(Vector3 touchPosition, float time)
    {
        startPos = touchPosition;
    }

    void MoveInput(Vector3 holdDirection)
    {
        holdDirection.Normalize();
        Vector3 camForward = Vector3.Scale(cameraMain.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveDir = (holdDirection.x * cameraMain.transform.right + holdDirection.y * camForward).normalized;
        moveActive = true;
    }

    // void Move(Vector3 worldPosition, float time)
    // {
    //     Vector3 projectPosition = (worldPosition - cameraMain.transform.position).normalized * camDist;
    //     projectPosition.z = camDist;
    //     Vector3 finalPos = cameraMain.transform.position + projectPosition;
    //     finalPos.y = rb.position.y;
    //     finalPos.z = rb.position.z;
    //
    //     rb.position = Vector3.Lerp(rb.position, finalPos, lerpSpeed* Time.deltaTime);
    // }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveTouch();
        KeyboardControls();
    }

    private void MoveTouch()
    {
        if (!moveActive) return;
        // print("addT: " +  rb.mass * movePower * moveDir);
        rb.AddForce( rb.mass * movePower * moveDir, ForceMode.Force);
    }

    void StopMove(Vector3 worldPosition, float time)
    {
        moveActive = false;
    }


    void Jump()
    {
        rb.AddForce(Vector3.up * minJump, ForceMode.Impulse);
    }

    void KeyboardControls()
    {
        int inputX = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        int inputY = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        if (inputX + inputY == 0) return;

        Vector3 camForward = Vector3.Scale(cameraMain.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirKB = (inputX * cameraMain.transform.right + inputY * camForward).normalized;
        
        // print("addKB: " +  rb.mass * movePower * moveDir);
        rb.AddForce(moveDirKB * rb.mass * movePower, ForceMode.Force);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }
    
    
}
