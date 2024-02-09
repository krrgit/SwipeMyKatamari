using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Size
    // Last Collected Prop
    // Remaining Props
    // Time Left
    [SerializeField] private float timer;
    [SerializeField] private float timeLimit = 180;
    [SerializeField] private bool gameRunning = true;
    [SerializeField] private int amountToWin;
    [SerializeField] private bool finished;
    
    [SerializeField]private Transform scenePropsParent;
    
    private PropCollector propCollector;
    private UIManager uiManager;
    private TouchManager touchManager;
    
    private void Awake()
    {
        propCollector = PropCollector.Instance;
        uiManager = UIManager.Instance;
        touchManager = TouchManager.Instance;
    }

    private void OnEnable()
    {
        propCollector.myCollectProp += GameStateCheck;
        touchManager.OnStartTouch += StartGame;
    }
    private void OnDisable()
    {
        propCollector.myCollectProp -= GameStateCheck;
        touchManager.OnStartTouch -= StartGame;
    }

    void StartGame(Vector3 postition, float time)
    {
        if (!gameRunning)
        {
            gameRunning = true;
            touchManager.OnStartTouch -= StartGame;
        }
    }
    
    void GameStateCheck(float newRadius, Prop prop)
    {
        if (!finished && gameRunning && scenePropsParent.childCount <= amountToWin)
        {
            finished = true;
            uiManager.ShowEndGameUI();
            SoundManager.Instance.PlayClip("Fanfare");
        }
        uiManager.UpdateUIText(newRadius, prop.PropName, scenePropsParent.childCount);
    }

    void Start()
    {
        timer = timeLimit;
        amountToWin = scenePropsParent.childCount / 2;
        uiManager.SetPropsMaxValue(scenePropsParent.childCount);
        uiManager.UpdateUIText(1, "", scenePropsParent.childCount);
        uiManager.UpdateTime(timer);
    }

    void Update()
    {
        if (!gameRunning) return;
        Tick();
    }

    void Tick()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            uiManager.UpdateTime(timer);
            
        }
        else
        {
            gameRunning = false;
            timer = 0;
            uiManager.UpdateTime(timer);
            // End Game
            uiManager.ShowGameOverUI();
        }
    }
}
