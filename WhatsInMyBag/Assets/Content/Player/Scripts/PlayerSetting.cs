using System;
using Bolt;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerSetting : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private OrbitCameraController cameraController;
    [SerializeField] private float cameraSize = 10;
    [SerializeField] private float cameraDistance = 20;
    [SerializeField] [Range(0, 89.9f)] private float cameraAngle = 45;
    [SerializeField] private float cameraRotation = 45;

    [Header("Input")]
    [SerializeField] private InputSettings inputSettings;
    [SerializeField] [Min(0)] private float mouseSensitivity = 1;
    [SerializeField] [Min(0)] private float controllerSensitivity = 3;

    private void Start()
    {
        if (cameraController)
        {
            cameraController.cameraSize = cameraSize;
            cameraController.angle = cameraAngle;
            cameraController.distance = cameraDistance;
            cameraController.targetRotation = cameraRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputSettings)
        {
            inputSettings.mouseSensitivity = mouseSensitivity;
            inputSettings.controllerSensitivity = controllerSensitivity;
        }
        
        if (Application.isPlaying) return;
        if (cameraController)
        {
            cameraController.cameraSize = cameraSize;
            cameraController.angle = cameraAngle;
            cameraController.distance = cameraDistance;
            cameraController.targetRotation = cameraRotation;
        }
    }
}
