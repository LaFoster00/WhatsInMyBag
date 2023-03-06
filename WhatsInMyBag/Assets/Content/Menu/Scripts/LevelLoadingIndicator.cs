using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoadingIndicator : MonoBehaviour
{
    [SerializeField] private LevelManagerData levelManagerData;
    [SerializeField] private Slider loadingIndicator;

    private void Awake()
    {
        if (!loadingIndicator)
        {
            loadingIndicator = GetComponent<Slider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        loadingIndicator.value = levelManagerData.loadingProgress;
    }
}
