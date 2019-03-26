using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bake : Tool
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI degreeText;

    private void Start()
    {
        OnSliderChanged();
    }

    public void OnSliderChanged(){
        slider.value = (int)(slider.value / 5) * 5;
        action.amount = (int)slider.value;
        degreeText.text = slider.value.ToString() + "°";
    }
}
