using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private RectTransform timerTransform;
    [SerializeField] private int fullWidth = 1328;
    [SerializeField] private TextMeshProUGUI timerLabel;

    public int timeInSeconds = 120;

    private float timer;
    private float timePercentage = 1;

    private void Start()
    {
        timer = timeInSeconds;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        timePercentage = (float)(timer / timeInSeconds);
        timerTransform.sizeDelta = new Vector2(fullWidth * timePercentage, timerTransform.sizeDelta.y);
        timerLabel.text = $"{(int)(timer / 60)}:{(int)(timer % 60)}";
    }
}
