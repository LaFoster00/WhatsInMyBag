using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CheckpointVFX : MonoBehaviour
{

    [SerializeField] private GameObject checkpoint;
    
    [SerializeField] private Collider trigger;
    [SerializeField] private Material checkpointBase;
    [SerializeField] private Material checkpointWall1;
    [SerializeField] private Material checkpointWall2;
    [SerializeField] private Material checkpointWall3;
    [SerializeField] private Material checkpointWall4;

    [SerializeField] private GameObject pulse;
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;
    [SerializeField] private GameObject wall4;



    [SerializeField] private GameObject defaultState;
    [SerializeField] private GameObject triggerState;
    [SerializeField] private GameObject activatedState;

    /*
    [Header("Animated Values: Material of GlitchPulse"), Space(20)]
    [SerializeField] private float rippleSpeed;
    [SerializeField] private float rippleDisplacementHeight;
    
    [SerializeField] private Color outlineColor;
    [SerializeField] private float outlineColorIntensity;
    
    [SerializeField] private Color rippleTopColor;
    [SerializeField] private float rippleTopColorIntensity;

    [SerializeField] private Color rippleBottomColor;
    [SerializeField] private float rippleBottomColorIntensity;



    [SerializeField] private float handAnimateRipple;
    [SerializeField] private bool autoAnimateRipple;
    [SerializeField] private bool showRipple;

    [SerializeField] private float overallAlpha;
    [SerializeField] private float innerGlowStrength;

    [Header("Animated Values: Material of Walls"), Space(20)] 
    [SerializeField] private Color color;
    [SerializeField] private float colorIntensity;

    [SerializeField] private float smokeClusterSize;
    [SerializeField] private float flameBrightness;
    [SerializeField] private float alpha;
    */
    
    
    
    
    
    
    
    private void Awake()
    {
        checkpointBase = pulse.GetComponent<MeshRenderer>().material;
        checkpointWall1 = wall1.GetComponent<MeshRenderer>().material;
        checkpointWall2 = wall2.GetComponent<MeshRenderer>().material;
        checkpointWall3 = wall3.GetComponent<MeshRenderer>().material;
        checkpointWall4 = wall4.GetComponent<MeshRenderer>().material;
        
        DefaultEffect();
    }

    private void OnEnable()
    {
        GameEventManager.AddListener<CheckpointActivatedEvent>(OnCheckpointActivated);
    }

    private void Start()
    {
        pulse.GetComponent<MeshRenderer>().material = checkpointBase;
        wall1.GetComponent<MeshRenderer>().material = checkpointWall1;
        wall2.GetComponent<MeshRenderer>().material = checkpointWall2;
        wall3.GetComponent<MeshRenderer>().material = checkpointWall3;
        wall4.GetComponent<MeshRenderer>().material = checkpointWall4;



        /*
        foreach (var wall in walls)
        {
            wall.GetComponent<MeshRenderer>().material = checkpointWall;
        }
        */
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener<CheckpointActivatedEvent>(OnCheckpointActivated);
    }

    private void Update()
    {
        /*
        if (Keyboard.current.kKey.IsPressed())
        {
            DefaultEffect();
        }
        if (Keyboard.current.lKey.IsPressed())
        {
            TriggerEffect();
        }
        if (Keyboard.current.mKey.IsPressed())
        {
            ActivatedEffect();
        }
        */
    }


    void OnCheckpointActivated(CheckpointActivatedEvent e)
    {
        if (e.Checkpoint == checkpoint)
        {
            TriggerEffect();
        }
    }
    
    
    private void DefaultEffect()
    {
        triggerState.GetComponent<PlayableDirector>().Stop();
        activatedState.GetComponent<PlayableDirector>().Stop();
        
        defaultState.GetComponent<PlayableDirector>().Play();
    }
    
    private void TriggerEffect()
    {
        defaultState.GetComponent<PlayableDirector>().Stop();
        activatedState.GetComponent<PlayableDirector>().Stop();

        triggerState.GetComponent<PlayableDirector>().Play();

        StartCoroutine(DelayEffect(1f));

    }
    
    private void ActivatedEffect()
    {
        triggerState.GetComponent<PlayableDirector>().Stop();
        defaultState.GetComponent<PlayableDirector>().Stop();
        
        activatedState.GetComponent<PlayableDirector>().Play();

    }

    IEnumerator DelayEffect(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ActivatedEffect();
    }
    
}
