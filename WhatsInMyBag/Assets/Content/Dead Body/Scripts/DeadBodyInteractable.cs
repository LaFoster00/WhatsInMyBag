using System;
using System.Collections;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;
using USCSL;

namespace DeadBody
{
    public class DeadBodyInteractable : MonoBehaviour, IInteractable
    {
        #region PROPERTIES

        [SerializeField] private float movementSpeed = 0.4f;
        [SerializeField] private float rotationSpeed = 0.2f;
        [SerializeField] private float3 localPositionOffset = new float3(0, 1, 0);
        [SerializeField] private float3 localRotationOffset = new float3(0, 180, 0);
        
        [SerializeField] private GameObject objectToHighlight;
        [SerializeField] private Transform boneToHold;

        [SerializeField] private AK.Wwise.Event deadBodyDragEvent;
        [SerializeField] private AK.Wwise.Event deadBodyDragStopEvent;
        [SerializeField] private AK.Wwise.RTPC deadBodyDragRTCP;

        private bool _isInteractable = true;

        public bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }

        public bool IsHighlighted
        {
            get => objectToHighlight.layer == LayerMask.NameToLayer("Outline");
        }

        public GameObject GameObject
        {
            get => gameObject;
        }

        public static DeadBodyInteractable Instance { get; private set; }

        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this) DestroyImmediate(gameObject);
            Instance = this;
            tag = "Interactable";
        }

        private void OnDisable()
        {
            deadBodyDragStopEvent.Post(gameObject);
        }

        public bool IsInteractableInContext(PlayerController controller)
        {
            return IsInteractable && (!controller.HeldBody && !controller.HeldItem && !controller.HeldProp);
        }

        public IEnumerator OnInteraction(InteractionData data)
        {
            if (data.PlayerController.HeldItem || data.PlayerController.HeldProp) yield break;

            _isInteractable = false;

            #region Pickup Body
            
            Rigidbody boneRigidBody = boneToHold.GetComponent<Rigidbody>();
            boneRigidBody.isKinematic = true;
            boneToHold.gameObject.layer = LayerMask.NameToLayer("No Player Collision");
            data.PlayerController.IsControllable = false;

            boneToHold.parent = data.PlayerController.Hand;

            Vector3 startingPosition = boneToHold.localPosition;
            quaternion startingRotation = boneToHold.localRotation;
            data.PlayerController.CharacterRotation = 180f;

            data.PlayerController.SetAnimatorValue("IsBodyGrabbing", true);

            float lerpTime = 0;
            while (lerpTime <= 0.2f)
            {
                lerpTime += Time.deltaTime;
                boneToHold.localPosition =
                    math.lerp(startingPosition, localPositionOffset / data.PlayerController.MahleeCharacter.localScale, lerpTime / 0.5f);
                boneToHold.localRotation = math.slerp(startingRotation, quaternion.Euler(math.radians(localRotationOffset)), lerpTime / 0.5f);
                yield return null;
            }

            boneToHold.localPosition = localPositionOffset /
                                      data.PlayerController.MahleeCharacter.transform.localScale;
            boneToHold.localRotation = quaternion.Euler(math.radians(localRotationOffset));

            data.PlayerController.IsControllable = true;
            data.PlayerController.MovementSpeedFactor = movementSpeed;
            data.PlayerController.RotationSpeedFactor = rotationSpeed;
            data.PlayerController.HeldBody = this;

            deadBodyDragEvent.Post(gameObject);
            deadBodyDragRTCP.SetValue(gameObject, 0);

            #endregion

            float3 lastPosition = transform.position;
            
            while (!data.Input.InteractDown)
            {
                float velocity = math.length((float3) transform.position - lastPosition) / Time.deltaTime;
                deadBodyDragRTCP.SetValue(gameObject,
                    math.clamp(math.remap(0.0f, data.PlayerController.TrueMovementSpeed, 0, 100, velocity), 0, 100));
                /*
                boneToHold.localPosition = localPositionOffset /
                                           data.PlayerController.MahleeCharacter.transform.localScale;
                boneToHold.localRotation = quaternion.Euler(math.radians(localRotationOffset));
                */
                lastPosition = transform.position;
                yield return null;
            }

            #region DropBody

            data.PlayerController.SetAnimatorValue("IsBodyGrabbing", false);

            #region Dispose Of Body

            if (data.PlayerController.NextInteractionTarget is DeadBodyDisposal disposal)
            {
                print("Disposing this mofo!");
                disposal.OnInteraction(data);
                yield break;
            }
            
            #endregion

            boneRigidBody.isKinematic = false;
            boneRigidBody.gameObject.layer = LayerMask.NameToLayer("Dead Body");
            boneToHold.parent = null;
            data.PlayerController.MovementSpeedFactor = 1.0f;
            data.PlayerController.RotationSpeedFactor = 1.0f;
            data.PlayerController.CharacterRotation = 0.0f;
            data.PlayerController.HeldBody = null;
            deadBodyDragStopEvent.Post(gameObject);

            #endregion
            
            _isInteractable = true;
        }
        
        private IEnumerator UpdateOffset(InteractionData data)
        {
            while (true)
            {
                
                yield return null;
            }
        }

        public void Highlight(PlayerController controller)
        {
            objectToHighlight.ChangeLayerRecursive(LayerMask.NameToLayer("Outline"));
        }

        public void RemoveHighlight(PlayerController controller)
        {
            objectToHighlight.ChangeLayerRecursive(LayerMask.NameToLayer("Default"));
        }
    }
}