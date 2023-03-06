using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

public class FanPaperstorm : MonoBehaviour
{
    [SerializeField] private GameObject interactable;
    [SerializeField] private GameObject startEffect;
    [SerializeField] private GameObject loopEffect;
    [SerializeField] private GameObject stopEffect;

    [SerializeField] private float effectDuration;
    
    private void OnEnable()
    {
        GameEventManager.AddListener<InteractionHappenedEvent>(OnInteractionHappenedEvent);
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        
    }

    private void OnInteractionHappenedEvent(InteractionHappenedEvent e)
    {
        if (interactable != null && e.PropInteractedWith.gameObject == interactable)
        {
            PlayEffect();
        }
    }
    
    private void PlayEffect()
    {
        loopEffect.GetComponent<PlayableDirector>().Stop();
        stopEffect.GetComponent<PlayableDirector>().Stop();

        startEffect.GetComponent<PlayableDirector>().Play();

        StartCoroutine(TransitionToLoop(2f));
    }

    private IEnumerator TransitionToLoop(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        startEffect.GetComponent<PlayableDirector>().Stop();
        stopEffect.GetComponent<PlayableDirector>().Stop();

        loopEffect.GetComponent<PlayableDirector>().Play();

        StartCoroutine(TransitionToStop(effectDuration));
    }
    
    private IEnumerator TransitionToStop(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        startEffect.GetComponent<PlayableDirector>().Stop();
        loopEffect.GetComponent<PlayableDirector>().Stop();

        stopEffect.GetComponent<PlayableDirector>().Play();
    }
}
