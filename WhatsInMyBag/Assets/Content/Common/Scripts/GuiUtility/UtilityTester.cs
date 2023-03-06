using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace GuiUtility
{

    public class UtilityTester : MonoBehaviour
    {
        [SerializeField] private RectTransform image;

        private Collider _collider;


        private void Awake()
        {
            _collider = gameObject.GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            image.position = GuiUtility.GetScreenPointAboveObject(_collider);
        }
    }
}