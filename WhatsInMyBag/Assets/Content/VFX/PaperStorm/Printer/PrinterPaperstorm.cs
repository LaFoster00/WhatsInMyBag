using System;
using System.Collections;
using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

public class PrinterPaperstorm : MonoBehaviour
{
    [SerializeField] private GameObject interactable;
    [SerializeField] private PlayableDirector startEffect;
    [SerializeField] private PlayableDirector loopEffect;
    [SerializeField] private PlayableDirector stopEffect;

    [SerializeField] private float effectDuration;
    
    private void OnEnable()
    {
        //GameEventManager.AddListener<InteractionHappenedEvent>(OnInteractionHappenedEvent);
    }

    private void OnDisable()
    {
        //GameEventManager.RemoveListener<InteractionHappenedEvent>(OnInteractionHappenedEvent);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        /*
        if (Keyboard.current.kKey.IsPressed())
        {
            PlayEffect();
        }
        */
    }

    private void OnInteractionHappenedEvent(InteractionHappenedEvent e)
    {
        if (interactable != null && e.PropInteractedWith.gameObject == interactable)
        {
            PlayEffect();
        }
    }

    public void PlayEffect(float timeBeforeTransition = 2.0f, bool endLoop = false)
    {
        loopEffect.Stop();
        stopEffect.Stop();

        startEffect.Play();

        StartCoroutine(TransitionToLoop(timeBeforeTransition, endLoop));
    }

    private IEnumerator TransitionToLoop(float seconds, bool endLoop = false)
    {
        yield return new WaitForSeconds(seconds);
        
        startEffect.Stop();
        stopEffect.Stop();

        loopEffect.Play();

        if (endLoop) StartCoroutine(TransitionToStop(effectDuration));
    }
    
    private IEnumerator TransitionToStop(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        startEffect.Stop();
        loopEffect.Stop();

        stopEffect.Play();
    }

    public void StopEffect()
    {
        startEffect.Stop();
        loopEffect.Stop();

        stopEffect.Play();
    }
}
