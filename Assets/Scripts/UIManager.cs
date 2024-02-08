using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TMP_Text size;
    [SerializeField] private TMP_Text timeLeft;
    [SerializeField] private TMP_Text lastPropCollected;
    [SerializeField] private TMP_Text propsLeft;
    [SerializeField] private Slider propsLeftSlider;
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    public void ShowEndGameUI()
    {
        endGameCanvas.SetActive(true);
    }

    public void ShowGameOverUI()
    {
        gameOverCanvas.SetActive(true);
    }

    public void SetPropsMaxValue(int maxValue)
    {
        propsLeftSlider.maxValue = maxValue;
        propsLeftSlider.value = maxValue;
    }
    


    public void UpdateUIText(float newRadius, string propName, int propCount)
    {
        size.text = newRadius.ToString("F2") + " cm";   
        lastPropCollected.text = propName;
        propsLeft.text = propCount + " left";
        propsLeftSlider.value = propCount;
    }

    public void UpdateTime(float newTime)
    {
        int totalSeconds = Mathf.FloorToInt(newTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timeLeft.text =  minutes.ToString() + ":" + seconds.ToString("00");
    }

    public void UpdateLastProp(string propName)
    {
        lastPropCollected.text = propName;
    }

    public void UpdatePropsLeft(int amount)
    {
        propsLeft.text = amount + " left";
    }
}
