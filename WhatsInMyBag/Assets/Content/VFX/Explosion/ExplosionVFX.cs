using System;
using System.Collections;
using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

public class ExplosionVFX : MonoBehaviour
{
    [SerializeField] private GameObject interactable;
    [SerializeField] private PlayableDirector explosion;
    [SerializeField] private PlayableDirector loop;
    [SerializeField] private PlayableDirector stop;

    [SerializeField] private float effectDuration = 20f;

    private bool _isActive = false;
    
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
        //explosion.SetActive(false);
    }

    private void OnInteractionHappenedEvent(InteractionHappenedEvent e)
    {
        if (interactable != null && e.PropInteractedWith.gameObject == interactable)
        {
            Explode(1.6f, true);
        }
    }


    private void Update()
    {
        /*
        if (Keyboard.current.kKey.IsPressed())
        {
            Explode();
        }
        */
    }

    public void Explode(float timeBeforeTransition = 1.6f, bool endLoop = false)
    {
        if (_isActive) return;
        explosion.gameObject.SetActive(true);
        loop.gameObject.SetActive(true);
        
        explosion.GetComponent<PlayableDirector>().Play();
        StartCoroutine(TransitionToLoop(timeBeforeTransition, endLoop));
    }

    private IEnumerator TransitionToLoop(float seconds, bool endLoop = false)
    {
        yield return new WaitForSeconds(seconds);
        stop.Stop();
        explosion.Stop();
        
        loop.Play();
        _isActive = true;
        
        if (endLoop) StartCoroutine(StopEffectAfterSeconds(effectDuration));
    }

    private IEnumerator StopEffectAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        explosion.Stop();
        loop.Stop();
        
        stop.Play();

        _isActive = false;

    }

    public void StopEffect()
    {
        explosion.Stop();
        loop.Stop();
        stop.Play();
        _isActive = false;
    }
}
