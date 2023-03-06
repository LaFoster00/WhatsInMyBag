using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New InputSettings", menuName = "Input/InputSettings")]
public class InputSettings : ScriptableObject
{
    [Min(0)] public float mouseSensitivity = 1;
    [Min(0)] public float controllerSensitivity = 3;

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }

    public void SetControllerSensitivity(float value)
    {
        controllerSensitivity = value;
    }
}
