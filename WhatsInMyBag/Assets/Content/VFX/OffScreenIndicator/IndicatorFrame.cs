using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorFrame : MonoBehaviour
{
    [SerializeField] public GameObject IndicatorArrow;
    [SerializeField] public GameObject IndicatorDistance;

    [SerializeField] public GameObject Head;
    [SerializeField] public GameObject PulseRings;



    private Image _image;
    private Text _text;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = IndicatorDistance.GetComponent<Text>();
    }

    public Image GetIcon()
    {
        return _image;
    }

    public void SetIndicatorDistanceText(string text)
    {
        _text.text = text;
    }
}
