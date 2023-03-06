using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceChangedRaiser : MonoBehaviour
{
    private string _lastControlScheme;
    public void OnControlsChangedEvent(PlayerInput input)
    {
        if (input.currentControlScheme != _lastControlScheme)
        {
            _lastControlScheme = input.currentControlScheme;
            GameEventManager.Raise(new InputDeviceChangedEvent(_lastControlScheme));
        }
    }
}
