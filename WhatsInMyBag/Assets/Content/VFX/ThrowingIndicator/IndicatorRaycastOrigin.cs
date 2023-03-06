using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class IndicatorRaycastOrigin : MonoBehaviour
{

    [SerializeField] private GameObject _raycastOriginGameObject;
    [SerializeField] private GameObject _indicatorVFX;


    private void Start()
    {
        //
    }

    private void Update()
    {
        //Physics.Raycast(_raycastOriginGameObject.transform.localToWorldMatrix, (0,-1,0), 50.0f)
    }
}
