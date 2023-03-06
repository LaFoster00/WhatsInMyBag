using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkStateTriggerFilter : MonoBehaviour
{
    [SerializeField] private string filterTag;
    [SerializeField] private AK.Wwise.State state;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(filterTag))
        {
            //print($"State {state.Name}");
            state.SetValue();
        }
    }
}
