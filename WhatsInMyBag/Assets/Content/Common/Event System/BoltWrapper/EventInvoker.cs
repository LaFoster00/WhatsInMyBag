using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class EventInvoker : MonoBehaviour
{
    private void Update()
    {
        GameEventManager.Raise(new InteractableImpactEvent(null, "Hello World"));
    }
}
