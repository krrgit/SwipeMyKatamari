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
    [Header("Jump Variables")]
    [SerializeField] private float minJump = 5;
    [SerializeField] private float maxJump = 15;
    [SerializeField] private float jumpChargeMaxTime = 5;
    [SerializeField] private AnimateJumpParticles jumpParticles;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    
    private Vector2 touchDir;

    private Vector3 moveDir;
    private bool moveActive;

    private bool isJumpHold;
    private Coroutine jumpHold;
    
    private Camera cameraMain;
    private TouchManager touchManager;

    private void Awake()
    {
        touchManager = TouchManager.Instance;
        cameraMain = Camera.main;
    }

    private void OnEnable()
    {
        touchManager.OnDragTouch += MoveInput;
        touchManager.OnEndTouch += StopMove;

        touchManager.OnBottomStartTouch += StartJumpHold;
        touchManager.OnBottomEndTouch += ReleaseJumpHold;
        touchManager.OnBottomCancelTouch += CancelJumpHold;
    }
    private void OnDisable()
    {
        touchManager.OnDragTouch -= MoveInput;
        touchManager.OnEndTouch -= StopMove;
        
        touchManager.OnBottomStartTouch -= StartJumpHold;
        touchManager.OnBottomEndTouch -= ReleaseJumpHold;
        touchManager.OnBottomCancelTouch -= CancelJumpHold;
    }

    void Start()
    {
        rb.maxAngularVelocity = maxAngularVelocity;
    }
    

    void MoveInput(Vector3 holdDirection)
    {
        holdDirection.Normalize();
        Vector3 camForward = Vector3.Scale(cameraMain.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveDir = (holdDirection.x * cameraMain.transform.right + holdDirection.y * camForward).normalized;
        moveActive = true;
    }

    void Update()
    {
        GroundCheck();
    }


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


    void ReleaseJumpHold()
    {
        isJumpHold = false;
    }

    void CancelJumpHold()
    {
        if (jumpHold != null)StopCoroutine(jumpHold);
        jumpParticles.StopAnim();
        SoundManager.Instance.StopClip("HoldJump");
        SoundManager.Instance.StopClip("ChargeJump");
    }

    void StartJumpHold()
    {
        if (!isGrounded) return;
        jumpHold = StartCoroutine(IJumpHold());
        jumpParticles.StartAnim(jumpChargeMaxTime);
    }

    IEnumerator IJumpHold()
    {
        SoundManager.Instance.PlayClip("ChargeJump");
        isJumpHold = true;
        float timer = jumpChargeMaxTime;
        while (timer > 0 && isJumpHold)
        {
            timer -= Time.deltaTime;
            timer = timer < 0 ? 0 : timer;
            yield return new WaitForEndOfFrame();
        }
        float ratio = (1f - timer) / jumpChargeMaxTime;
        float jumpPower = minJump + (ratio * (maxJump - minJump));
        
        SoundManager.Instance.PlayClip("HoldJump");
        
        // Only jump when tap is released
        while (isJumpHold)
        {
            yield return new WaitForEndOfFrame();
        }
        
        // Jump
        jumpParticles.StopAnim();
        rb.AddForce(jumpPower *Vector3.up, ForceMode.Impulse);
        isJumpHold = false;

        SoundManager.Instance.StopClip("ChargeJump");
        SoundManager.Instance.StopClip("HoldJump");
        SoundManager.Instance.PlayClip("Jump");
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
    
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    
    
}
