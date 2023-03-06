using System;
using System.Collections.Generic;
using Bolt;
using Interactables.Items;
using Interactables.Props.Data;
using TypeReferences;
using Unity.Mathematics;
using UnityEngine;

namespace Interactables.Props
{
    [Serializable]
    public struct ItemInteraction
    {
        public ItemDescription item;
        public FlowMacro interaction;

        public bool IsValid()
        {
            return item && interaction;
        }
    }

    [CreateAssetMenu(fileName = "New PropDescription", menuName = "Interactable/PropDescription")]
    public class PropDescription : ScriptableObject
    {
        [Header("Grabing")]
        public float3 localGrabOffset = new float3(0, 0.75f, 0);
        public float3 localRotationOffset = new float3(-90, 0, 90);

        [Header("Interaction")]
        public PropInteraction behaviour = PropInteraction.Pickup;
        public float weight = 1;
        [Tooltip("Only Needed if behaviour is set to Interaction.")]
        public FlowMacro interaction;
        
        [Header("Throwing")] 
        public bool emitEventOnImpact = false;
        public string eventIdentifier = "ItemImpact";
        public float impactEventSpeedThreshold = 5;
        public bool emitSoundOnImpact = false;
        public AK.Wwise.Event impactAudioEvent;

        [Header("Alertion")] 
        public float minAlertionDistance = 1;
        public float maxAlertionDistance = 5;
        public float alertionAngle = 180;
        public float alertionDirection = 0;
        public bool debugViewAlertionCone = true;
        
        #region NON_ITEM_BEHAVIOUR

        public List<ItemInteraction> itemInteractions;

        #endregion
        
        [Header("Data")]
        [Inherits(typeof(IPropData))] public TypeReference customDataType;
    }
}