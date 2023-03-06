using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulseRing : MonoBehaviour
{
    // This value is animated by the timeline
    [SerializeField, Range(0,1)] public float alpha;
    
    public void UpdateColor(Vector3 rbgNewColor)
    {
        this.GetComponent<Image>().color = new Color(rbgNewColor.x, rbgNewColor.y, rbgNewColor.z, alpha);
    }
}
