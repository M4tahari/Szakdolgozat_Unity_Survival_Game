using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySliderValue : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Text sliderText;

    public void Update()
    {
        sliderValue = slider.value;
        sliderText.text = sliderValue.ToString("0.00");
    }
}
