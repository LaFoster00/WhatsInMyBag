using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceVFX : MonoBehaviour
{
    [SerializeField] public GameObject Start;
    [SerializeField] public GameObject CloseBy;
    [SerializeField] public GameObject Trigger;
    [SerializeField] public GameObject Cancel;


    private void Awake()
    {
        Start.SetActive(false);
        CloseBy.SetActive(false);
        Trigger.SetActive(false);
        Cancel.SetActive(false);
    }
    
}
