using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionSign : MonoBehaviour
{
    [SerializeField] private float Amplitude = default;
    [SerializeField] private float frequency = default;
    [SerializeField] private float SpinSpeed = 1;

    private Vector3 startPosition;
    private Vector3 currentPosition;
    
    void OnEnable()
    {
        startPosition = this.transform.position;
        currentPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition.y = startPosition.y + Amplitude * Mathf.Sin(Time.time * frequency) ;
        this.transform.position = currentPosition;

        transform.Rotate(0f, 0f, 100f * Time.deltaTime * SpinSpeed) ;
        
    }
}
