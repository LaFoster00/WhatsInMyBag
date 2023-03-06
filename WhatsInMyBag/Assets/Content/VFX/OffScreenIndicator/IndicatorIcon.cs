using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorIcon : MonoBehaviour
{
    [SerializeField] public Sprite IndicatorImage;
    [SerializeField] public Vector2 IconOffset = Vector2.zero;
    [SerializeField] public Vector3 IconScale = Vector3.one;
    [SerializeField] public Vector3 PopupOffset = 200f * Vector3.right + 50f * Vector3.up;
}
