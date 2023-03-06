using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bolt;
using GameEvents;
using Interactables.Bolt;
using Interactables.Props.Data;
using Ludiq;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;
using USCSL;

namespace Interactables.Props
{
    public enum PropInteraction
    {
        None = 0,
        Interact = 1,
        Pickup = 2,
        Throwable = 3
    }

    [RequireComponent(typeof(FlowMachine))]
    public class Prop : MonoBehaviour, IInteractable
    {
        [SerializeField] private PropDescription description;
        public PropDescription Description => description;

        [HideInInspector] public new Rigidbody rigidbody;
        private bool _intectionAvailable;
        private IPropData _customData;
        private int _fallbackLayer;
        private FlowMachine _interactionFlowMachine;
        private GraphReference _graphReference;

        private float _rigidbodyMass;
        private bool _isRigidbodyKinematic;
        private bool _rigidibodyCollision;
            
        [SerializeField] private bool isInteractable = true;
        public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
        public bool IsHighlighted { get => gameObject.layer == LayerMask.NameToLayer("Outline"); }
        public GameObject GameObject { get => gameObject; }

        private void Awake()
        {
            gameObject.tag = "Interactable";
            rigidbody = GetComponent<Rigidbody>();

            if ((description.behaviour == PropInteraction.Pickup ||
                 description.behaviour == PropInteraction.Throwable) && !rigidbody)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            else if (rigidbody && gameObject.isStatic)
            {
                Destroy(rigidbody);
            }
            
            if (description && description.customDataType !=null && description.customDataType.Type != null)
            {
                _customData = (IPropData) Activator.CreateInstance(description.customDataType.Type);
            }
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            _fallbackLayer = gameObject.layer;
            
            _interactionFlowMachine = GetComponent<FlowMachine>();
            if (!_interactionFlowMachine) _interactionFlowMachine = gameObject.AddComponent<FlowMachine>();
        }

        private void OnEnable()
        {
            GameEventManager.AddListener<ItemPickedUpEvent>(OnItemPickedUp);
            GameEventManager.AddListener<ItemDroppedEvent>(OnItemDropped);
        }

        private void OnDisable()
        {
            GameEventManager.RemoveListener<ItemPickedUpEvent>(OnItemPickedUp);
            GameEventManager.RemoveListener<ItemDroppedEvent>(OnItemDropped);
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (description.debugViewAlertionCone && description.minAlertionDistance > 0)
            {
                float3 forwardDistance = math.mul(quaternion.Euler(0, math.radians(description.alertionDirection), 0),transform.forward * description.maxAlertionDistance);
                float3 leftBoundary = math.mul(quaternion.Euler(0, math.radians(description.alertionAngle / 2), 0), forwardDistance);
                float3 rightBoundary = math.mul(quaternion.Euler(0, math.radians(-description.alertionAngle / 2), 0), forwardDistance);
                
                Debug.DrawLine(transform.position, transform.position + (Vector3)forwardDistance, Color.green);
                Debug.DrawLine(transform.position, transform.position + (Vector3)leftBoundary, Color.magenta);
                Debug.DrawLine(transform.position, transform.position + (Vector3)rightBoundary, Color.blue);
            }
            #endif

            if (isInteractable)
            {
                if (gameObject.layer == 0)
                {
                    gameObject.ChangeLayerRecursive(_fallbackLayer);
                }
            }
            else if (gameObject.layer != 0)
            {
                gameObject.ChangeLayerRecursive(0);
            }

            if (_graphReference == null) ;
        }

        private void OnItemPickedUp(ItemPickedUpEvent @event)
        {
            if (isInteractable && description && description.itemInteractions.FirstOrDefault(i =>
                i.item == @event.Controller.HeldItem.description).IsValid())
            {
                _intectionAvailable = true;
                GameEventManager.Raise(new PropInteractionAvailableEvent(this, @event.Controller));
            }
        }

        private void OnItemDropped(ItemDroppedEvent @event)
        {
            _intectionAvailable = false;
        }

        public bool IsInteractableInContext(PlayerController controller)
        {
            return isInteractable && (!controller.HeldItem && !controller.HeldProp && !controller.HeldBody && description.behaviour != PropInteraction.None || _intectionAvailable);
        }

        public IEnumerator OnInteraction(InteractionData data)
        {
            if (data.PlayerController.HeldItem)
            {
                isInteractable = false;
                if (description.itemInteractions.Count >= 0)
                {
                    ItemInteraction interaction = description.itemInteractions.FirstOrDefault(i =>
                        i.item == data.PlayerController.HeldItem.description);

                    if (interaction.IsValid())
                    {
                        _interactionFlowMachine.nest.SwitchToMacro(interaction.interaction);
                        _graphReference = (GraphReference) _interactionFlowMachine.GetReference();
                        GameEventManager.Raise(new InteractionHappenedEvent(this));
                        _graphReference.TriggerEventHandler(
                            hook => hook.name == "OnItemInteract",
                            new ItemInteractionEventArgs(data.PlayerController, data.PlayerController.HeldItem, this),
                            _ => false,
                            false);

                        while (true)
                        {
                            yield return new WaitForSeconds(float.MaxValue);
                        }
                    }
                }
                isInteractable = true;
                print("Cant interact with this item and that object!");
            }
            else if (!data.PlayerController.HeldProp && !data.PlayerController.HeldBody)
            {
                isInteractable = false;

                float prevMovementSpeedFactor = data.PlayerController.MovementSpeedFactor;
                float prevRotationSpeedFactor = data.PlayerController.RotationSpeedFactor;
                switch (description.behaviour)
                {
                    case PropInteraction.None:
                        isInteractable = true;
                        yield break;
                    case PropInteraction.Interact:
                        _interactionFlowMachine.nest.SwitchToMacro(description.interaction); 
                        _graphReference = (GraphReference) _interactionFlowMachine.GetReference();
                        _graphReference.TriggerEventHandler(
                            hook => hook.name == "OnPropInteract",
                            new PropInteractionEventArgs(data.PlayerController, this),
                            _ => false,
                            false);
                        while (true)
                        {
                            yield return null;
                        }
                        break;
                    case PropInteraction.Pickup:
                    case PropInteraction.Throwable:
                        data.PlayerController.GrabProp(this);
                        break;
                }
                
                while (!data.Input.InteractDown)
                {
                    transform.localPosition = Description.localGrabOffset / data.PlayerController.MahleeCharacter.localScale;
                    transform.localRotation = quaternion.Euler(math.radians(Description.localRotationOffset));
                    yield return null;   
                }
                
                switch (description.behaviour)
                {
                    case PropInteraction.Pickup:
                        data.PlayerController.ReleaseGrabbedProp(false, prevMovementSpeedFactor, prevRotationSpeedFactor);
                        break;
                    case PropInteraction.Throwable:
                        data.PlayerController.ReleaseGrabbedProp(true, prevMovementSpeedFactor, prevRotationSpeedFactor);
                        break;
                }

                isInteractable = true;
            }
        }

        public void Highlight(PlayerController controller)
        {
            _fallbackLayer = gameObject.layer;
            gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("Outline"));
        }

        public void RemoveHighlight(PlayerController controller)
        {
            gameObject.ChangeLayerRecursive(_fallbackLayer);
        }

        public void ChangeFallbackLayer(string newFallbackLayer)
        {
            _fallbackLayer = LayerMask.NameToLayer(newFallbackLayer);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (description && description.emitEventOnImpact)
            {
                if (collision.collider.CompareTag("Player")) return;
                
                float3 relativeVelocity = collision.relativeVelocity;
                float relativeVelocityMagnitude = math.length(relativeVelocity);
                if (relativeVelocityMagnitude >= description.impactEventSpeedThreshold)
                {
                    //Debug.Log(name + relativeVelocityMagnitude);
                    if (description.emitSoundOnImpact && description.impactAudioEvent.IsValid()) description.impactAudioEvent.Post(gameObject); 
                    GameEventManager.Raise(new InteractableImpactEvent(this, description.eventIdentifier));
                }
            }
        }

        public void EnableRigidbody()
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbodyMass = rigidbody.mass;
            _isRigidbodyKinematic = rigidbody.isKinematic;
            _rigidibodyCollision = rigidbody.detectCollisions;
        }

        public void DisableRigidbody()
        {
            rigidbody.mass = _rigidbodyMass;
            rigidbody.isKinematic = _isRigidbodyKinematic;
            rigidbody.detectCollisions = _rigidibodyCollision;
            Destroy(rigidbody);
            rigidbody = null;
        }

        public bool GetCustomDataGeneric<T>(out T data) where T : IPropData
        {
            if (_customData is T propData)
            {
                data = propData;
                return true;
            }

            data = null;
            return false;
        }

        public bool GetCustomDataObject(out object data, out Type type)
        {
            if (_customData != null)
            {
                data = _customData;
                type = description.customDataType;
                return true;
            }

            data = null;
            type = null;
            return false;
        }

        public static implicit operator PropDescription(Prop prop) => prop.description;

        public class GetCustomData : Unit
        {
            [Serialize, Inspectable, UnitHeaderInspectable] public Type customDataType;
            [DoNotSerialize] public ValueInput Prop;
            [DoNotSerialize] public Dictionary<string, ValueOutput> Outputs;
        
            [DoNotSerialize] public ControlInput Input;
            [DoNotSerialize] public ControlOutput Output;
            [DoNotSerialize] public ValueOutput PropOut;

            protected override void Definition()
            {
                PropOut = ValueOutput<Prop>("PropOut");
                Prop = ValueInput<Prop>("Prop");
                Outputs = new Dictionary<string, ValueOutput>();

                if (customDataType != null && customDataType.IsSubclassOf(typeof(IPropData)))
                {
                    FieldInfo[] fields = customDataType.GetFields();
                    foreach (var field in fields)
                    {
                        Outputs.Add(field.Name, ValueOutput(field.FieldType, field.Name));
                    }
                }

                Input = ControlInput("", Enter);
                Output = ControlOutput("");

                Requirement(Prop, Input);
            }
        
            public ControlOutput Enter(Flow flow)
            {
                Prop prop = flow.GetValue<Prop>(Prop);
                flow.SetValue(PropOut, prop);
                
                PropDescription propDescription = prop.description;
                if (propDescription.customDataType.Type == customDataType)
                {
                    foreach (var field in customDataType.GetFields())
                    {
                        flow.SetValue(Outputs[field.Name], field.GetValue(prop._customData));
                    }

                    return Output;
                }

                return null;
            }
        }
        
        public class SetCustomData : Unit
        {
            [Serialize, Inspectable, UnitHeaderInspectable] public Type customDataType;
            [DoNotSerialize] public ValueInput Prop;
            [DoNotSerialize] public Dictionary<string, ValueInput> Inputs;

            [DoNotSerialize] public ControlInput Input;
            [DoNotSerialize] public ControlOutput Output;
            [DoNotSerialize] public ValueOutput PropOut;
            
            protected override void Definition()
            {
                Output = ControlOutput("");
                PropOut = ValueOutput<Prop>("PropOut");
                Prop = ValueInput<Prop>("Prop");
                Inputs = new Dictionary<string, ValueInput>();

                if (customDataType != null && customDataType.IsSubclassOf(typeof(IPropData)))
                {
                    FieldInfo[] fields = customDataType.GetFields();
                    foreach (var field in fields)
                    {
                        Inputs.Add(field.Name, ValueInput(field.FieldType, field.Name));
                    }
                }

                Input = ControlInput("", Enter);

                Requirement(Prop, Input);
            }
        
            public ControlOutput Enter(Flow flow)
            {
                Prop prop = flow.GetValue<Prop>(Prop);
                flow.SetValue(PropOut, prop);
                
                PropDescription propDescription = prop.description;
                if (propDescription.customDataType.Type == customDataType)
                {
                    foreach (var field in customDataType.GetFields())
                    {
                        ValueInput input = Inputs[field.Name];
                        if (input.hasValidConnection || input.hasDefaultValue)
                        {
                            field.SetValue(prop._customData, flow.GetValue(input));
                        }
                    }

                    return Output;
                }

                return null;
            }
        }
    }
}