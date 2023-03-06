using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Controller
{
    public class CharacterLookAtController : MonoBehaviour
    {
        [SerializeField] private GameObject cameraLookAt;
        [SerializeField] private float maxDistance = 5;
        [SerializeField] private float speed = 1;
        
        private Transform _lookAtTransform;
        
        private void Start()
        {
            cameraLookAt.transform.position = transform.forward * maxDistance + transform.position;
            _lookAtTransform = cameraLookAt.transform;
        }

        // Update is called once per frame
        public void UpdateLookAtPosition(float3 movementDirection)
        {
            
            _lookAtTransform.position = math.lerp(
                _lookAtTransform.position,
                movementDirection * maxDistance + (float3)transform.position,
                speed * Time.deltaTime);
        }
    }
}