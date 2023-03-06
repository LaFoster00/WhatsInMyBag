using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    [Serializable]
    private struct WwiseAnimationEvent
    {
        public string EventSignature;
        public GameObject TargetObject;
        public AK.Wwise.Event WwiseEvent;
        public AK.Wwise.Switch WwiseSwitch;
        public AK.Wwise.State WwiseState;
    }

    [SerializeField] private List<WwiseAnimationEvent> wwiseAnimationEvents = new List<WwiseAnimationEvent>();

    private Dictionary<string, WwiseAnimationEvent> _wwiseAnimationEvents;

    private void Awake()
    {
        _wwiseAnimationEvents = wwiseAnimationEvents.ToDictionary(@event => @event.EventSignature);
    }

    private void WwiseEvent(string s)
    {
        if (_wwiseAnimationEvents.ContainsKey(s))
        {
            WwiseAnimationEvent @event = _wwiseAnimationEvents[s];
            GameObject target = @event.TargetObject ? @event.TargetObject : gameObject;
            if (@event.WwiseEvent.IsValid()) @event.WwiseEvent.Post(target);
            if (@event.WwiseSwitch.IsValid()) @event.WwiseSwitch.SetValue(target);
            if (@event.WwiseState.IsValid()) @event.WwiseState.SetValue();
        }
    }
}
