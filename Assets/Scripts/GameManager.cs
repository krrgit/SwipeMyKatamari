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
    
    [SerializeField]private Transform scenePropsParent;
    private PropCollector propCollector;
    private UIManager uiManager;
    
    private void Awake()
    {
        propCollector = PropCollector.Instance;
        uiManager = UIManager.Instance;
    }

    private void OnEnable()
    {
        propCollector.myCollectProp += GameStateCheck;
    }
    private void OnDisable()
    {
        propCollector.myCollectProp -= GameStateCheck;
    }
    
    void GameStateCheck(float newRadius, string propName)
    {
        if (gameRunning && scenePropsParent.childCount == 0)
        {
            gameRunning = false;
            uiManager.ShowEndGameUI();
        }
        uiManager.UpdateUIText(newRadius, propName, scenePropsParent.childCount);
    }

    void Start()
    {
        timer = timeLimit;
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
