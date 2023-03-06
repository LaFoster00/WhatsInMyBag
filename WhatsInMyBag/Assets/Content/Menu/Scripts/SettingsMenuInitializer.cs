using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuInitializer : MonoBehaviour
{
    [SerializeField] private InputSettings inputSettings;
    [SerializeField] private AudioSettings audioSettings;
    
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider controllerSensitivitySlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    private void OnEnable()
    {
        volumeSlider.value = audioSettings.volume;
        controllerSensitivitySlider.value = inputSettings.controllerSensitivity;
        mouseSensitivitySlider.value = inputSettings.mouseSensitivity;
    }
}
