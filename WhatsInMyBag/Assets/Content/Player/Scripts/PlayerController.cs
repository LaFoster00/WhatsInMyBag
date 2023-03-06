using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using DeadBody;
using GameEvents;
using Interactables.Items;
using Interactables.Props;
using Ludiq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using USCSL;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Player.Controller
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(FlowMachine))]
    [RequireComponent(typeof(CharacterLookAtController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        #region PROPERTIES
        
        [Header("Camera")]
        [SerializeField] private OrbitCameraController cameraController;

        #region Animation

        private SmoothDampFloat _animationMoveSpeed = new SmoothDampFloat();

        #endregion
        
        #region Bolt
        private FlowMachine _flowMachine;
        private GraphReference _graphReference;
        #endregion
        
        #region Character
        private Transform _character;
        private float _characterRotation;

        private Coroutine _rotationCoroutine;
        public float CharacterRotation
        {
            get => _characterRotation;
            set
            {
                _characterRotation = value;
                if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
                RotateCharacterTowards(value);
            }
        }
        #endregion
        
        #region Controlls
        [Header("Controls")]
        [SerializeField] private float movementSpeed = 4;
        public float MovementSpeed => movementSpeed;
        public float TrueMovementSpeed => movementSpeed * _movementSpeedFactor;

        private float _previousMovementSpeedFactor = 1;
        private float _movementSpeedFactor = 1;

        private float _previousRotationSpeedFactor = 1;
        private float _rotationSpeedFactor = 1;
        
        private float3 _normalizedMovement;
        private float3 _oldPosition;
        private float3 _currentMovement;

        public float MovementSpeedFactor
        {
            get => _movementSpeedFactor;
            set
            {
                if (!_canMove)
                {
                    _previousMovementSpeedFactor = value;
                }
                else
                {
                    _movementSpeedFactor = value;
                }
            }
        }

        public float RotationSpeedFactor
        {
            get => _rotationSpeedFactor;
            set
            {
                if (!_canMove)
                {
                    _previousRotationSpeedFactor = value;
                }
                else
                {
                    _rotationSpeedFactor = value;
                }
            }
        }

        private bool _canMove = true;
        public bool CanMove
        {
            get => _canMove;
            set
            {
                if (_canMove == value) return;
                if (value)
                {
                    _movementSpeedFactor = _previousMovementSpeedFactor;
                    _rotationSpeedFactor = _previousRotationSpeedFactor;
                }
                else
                {
                    _previousMovementSpeedFactor = _movementSpeedFactor;
                    _movementSpeedFactor = 0;
                    _previousRotationSpeedFactor = _rotationSpeedFactor;
                    _rotationSpeedFactor = 0;
                }

                _canMove = value;
            }
        }

        public bool CanInteract { get; set; } = true;

        public bool IsControllable
        {
            get => CanMove;
            set
            {
                CanMove = value;
                CanInteract = value;
            }
        }

        private float2 _movementDirection;
        private float _viewAxisValue;
        private CharacterLookAtController _lookAtController;
        #endregion

        #region Interaction
        [Header("Interaction")]
        [SerializeField] private Transform grabParent;
        public Transform GrabParent => grabParent;
        
        [SerializeField] private Transform hand;
        public Transform Hand => hand;

        [SerializeField] private Transform mahleeCharacter;
        public Transform MahleeCharacter => mahleeCharacter;

        private bool _interacting = false;
        
        private Dictionary<GameObject, IInteractable> _interactablesInReach = new Dictionary<GameObject, IInteractable>();
        private IInteractable _cr;

        private IInteractable _currentInteractionTarget
        {
            get => _cr;
            set => _cr = value;
        }
        private IInteractable _nextInteractionTarget;
        public IInteractable NextInteractionTarget => _nextInteractionTarget;

        private Coroutine _interaction;
        
        private GameObject _interactionHandler;
        private FlowMachine _interactionFlowMachine;
        public FlowMachine InteractionFlowMachine => _interactionFlowMachine;
        public GraphReference InteractionGraphReference => (GraphReference) _interactionFlowMachine.GetReference();

        private InteractionInput _input = new InteractionInput();
        #endregion
        
        #region Inventory
        private Item _heldItem;
        public Item HeldItem => _heldItem;

        private Prop _heldProp;
        public Prop HeldProp => _heldProp;
        
        public DeadBodyInteractable HeldBody;
        #endregion

        #region Internal
        private Camera _mainCamera;
        [SerializeField] private Animator animator;
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;
        #endregion

        #region Input
        [Header("Input")] 
        [SerializeField] private InputSettings inputSettings;
        private PlayerInput _playerInput;
        private bool _LastInteractButton;
        private bool _rotateView;
        #endregion

        #region Wwise

        [SerializeField] private AK.Wwise.Event dropEvent;
        [SerializeField] private AK.Wwise.Event grabEvent;
        [SerializeField] public AK.Wwise.Event throwEvent;

        #endregion
        
        #endregion

        private void Awake()
        {
            _character = transform.GetChild(0);
            _interactionHandler = Instantiate(new GameObject("Interaction Handler"), transform);
            _interactionFlowMachine = _interactionHandler.AddComponent<FlowMachine>();
            _playerInput = GetComponent<PlayerInput>();
            _lookAtController = GetComponent<CharacterLookAtController>();
            _rigidbody = GetComponent<Rigidbody>();
            GameManager.Instance.playerInput = _playerInput;
            
            Time.fixedDeltaTime = 1.0f / Screen.currentResolution.refreshRate;
        }

        private void Start()
        {
            _flowMachine = GetComponent<FlowMachine>();
            _graphReference = (GraphReference) _flowMachine.GetReference();
            
        }

        private void Update()
        {
            cameraController.targetRotation += _viewAxisValue * Time.deltaTime;
            _lookAtController.UpdateLookAtPosition(_normalizedMovement);
            
            UpdateInteractables();
            UpdateAnimator();
            UpdateInteractionInput();
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                if (_interactablesInReach.ContainsKey(other.gameObject) || _heldItem && other.gameObject == _heldItem.gameObject) return; // we dont want to add the currently held object back into the possible selection
                
                IInteractable interactable = other.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _interactablesInReach.Add(other.gameObject, interactable);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactablesInReach.ContainsKey(other.gameObject))
            {
                _interactablesInReach.Remove(other.gameObject);
            }
        }

        #region INPUT

        public void OnInteract(InputAction.CallbackContext context)
        {
            _input.Interact = context.performed;
            
            if (!CanInteract) return;
            
            if (_currentInteractionTarget == null)
            {
                if (!context.started) return;
                
                if (_nextInteractionTarget == null)
                {
                    if (context.started && _heldItem)
                    {
                        _currentInteractionTarget = _heldItem;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    _currentInteractionTarget = _nextInteractionTarget;
                }
            }

            if (!_interacting)
            {
                if (context.started)
                {
                    _interaction = StartCoroutine(InteractCoroutine(_currentInteractionTarget));
                }
            }
        }
        
        public void OnMovement(InputAction.CallbackContext context)
        {
            _movementDirection = context.ReadValue<Vector2>();
            CustomEvent.Trigger(gameObject, "On" + context.action.name, context.ReadValue<Vector2>());
        }

        public void OnView(InputAction.CallbackContext context)
        {
            if (_rotateView || _playerInput.currentControlScheme != "KeyboardMouse")
            {
                _viewAxisValue = context.ReadValue<float>() * (_playerInput.currentControlScheme == "KeyboardMouse"
                    ? inputSettings.mouseSensitivity
                    : inputSettings.controllerSensitivity);
            }
            else
            {
                _viewAxisValue = 0;
            }

            CustomEvent.Trigger(gameObject, "On" + context.action.name, _viewAxisValue);
        }

        public void OnRotateView(InputAction.CallbackContext context)
        {
            _rotateView = context.performed;
        }

        public void UpdateInteractionInput()
        {
            _input.InteractDown = _input.Interact && _input.Interact != _LastInteractButton;
            _input.InteractUp = !_input.Interact && _input.Interact != _LastInteractButton;
            _LastInteractButton = _input.Interact;
        }
        
        #endregion

        #region Interaction

        public void PickUpItem(Item item, bool releaseHeldItem = false, bool destroyReleasedItem = false)
        {
            if (_heldItem && releaseHeldItem)
            {
                DropHeldItem(destroyReleasedItem);
            }
            
            if (!_heldItem && item)
            {
                _heldItem = item;
                Transform itemTransform = item.transform;
                itemTransform.parent = grabParent;
                item.transform.localPosition = item.description.localGrabOffset / MahleeCharacter.transform.localScale;
                item.transform.localRotation = quaternion.Euler(math.radians(item.description.localRotationOffset));
                item.IsInteractable = false;
                item.SetKinematic(true, false);
                _currentInteractionTarget = null;
                item.gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("No Player Collision"));
                SetAnimatorValue("IsHolding", true);

                GameEventManager.Raise(new ItemPickedUpEvent(_heldItem, this));
                grabEvent.Post(gameObject);

                print($"Picked up item {item.name}");
            }
        }
        
        public void DropHeldItem(bool destroy = false)
        {
            if (!_heldItem) return;
            //we dont need to add the prop back into the list of possible items as we change its rigidbody which
            //will cause a trigger event in the next physics update by which point we dont hold it anymore anyways

            print($"Droped held item {_heldItem.name}");

            if (destroy)
            {
                DiscardHeldItem();
            }
            else
            {
                _heldItem.SetKinematic(false);
                _heldItem.transform.parent = null;
                _heldItem.IsInteractable = true;
                _heldItem = null;
                SetAnimatorValue("IsHolding", false);
                dropEvent.Post(gameObject);
            }
            
            GameEventManager.Raise(new ItemDroppedEvent(_heldItem, this));
        }

        public void GrabProp(Prop prop)
        {
            prop.transform.parent = grabParent;
            prop.transform.localPosition = prop.Description.localGrabOffset / MahleeCharacter.transform.localScale;
            prop.transform.localRotation = quaternion.Euler(math.radians(prop.Description.localRotationOffset));
            prop.DisableRigidbody();
            MovementSpeedFactor = 1.0f / (prop.Description.weight > 0 ? prop.Description.weight : 1f);
            SetAnimatorValue("IsHolding", true);
            _heldProp = prop;
            grabEvent.Post(gameObject);

            print($"(Grabed Prop {prop.name}");
        }

        public void ReleaseGrabbedProp(bool throwProp, float newMovementSpeedFactor, float newRotationSpeedFactor)
        {
            print($"(Released Prop {HeldProp.name}");
            HeldProp.EnableRigidbody();
            HeldProp.transform.parent = null;
            if (throwProp)
            {
                HeldProp.rigidbody.AddForce(
                    (transform.forward + new Vector3(0, 1, 0)) / _heldProp.Description.weight * 10,
                    ForceMode.Impulse);
                HeldProp.rigidbody.AddTorque(new float3(Random.Range(-40, 40), Random.Range(-40, 40),
                    Random.Range(-40, 40)));
                throwEvent.Post(gameObject);
            }
            MovementSpeedFactor = newMovementSpeedFactor;
            RotationSpeedFactor = newRotationSpeedFactor;
            _heldProp = null;
            dropEvent.Post(gameObject);

            SetAnimatorValue("IsHolding", false);
        }

        private void UpdateInteractables()
        {
            IInteractable minDistanceInteractable = GetNearestInteractable(this, transform, _interactablesInReach);
            float angleTowardsInteractable =
                minDistanceInteractable != null ?
                math.dot((minDistanceInteractable.GameObject.transform.position - transform.position).normalized, transform.forward) :
                0;
            if (angleTowardsInteractable < 0) minDistanceInteractable = null;
            
            if (_nextInteractionTarget != minDistanceInteractable)
            {
                if (_nextInteractionTarget != null)
                {
                    _nextInteractionTarget.RemoveHighlight(this);
                    GameEventManager.Raise(new ExitingViableInteractableRangeEvent(_nextInteractionTarget.GameObject.gameObject));
                    print($"Next Interaction Target is no longer {_nextInteractionTarget.GameObject.name}");
                }

                _nextInteractionTarget = minDistanceInteractable;
            }
            
            if (_nextInteractionTarget is {IsHighlighted: false})
            {
                _nextInteractionTarget.Highlight(this);
                GameEventManager.Raise(new EnteringViableInteractableRangeEvent(_nextInteractionTarget.GameObject.gameObject));
                print($"Next Interaction Target is {_nextInteractionTarget.GameObject.name}");
            }
        }

        public void EndInteraction(bool makeTargetInteractable = false)
        {
            if (_interacting)
            {
                if (_interaction != null)
                {
                    StopCoroutine(_interaction);
                }

                _interaction = null;
                _interacting = false;
                
                if (makeTargetInteractable && _currentInteractionTarget != null && _currentInteractionTarget.GameObject)
                {
                    _currentInteractionTarget.IsInteractable = true;
                }

                _currentInteractionTarget = null;
            }
        }

        public IEnumerator InteractCoroutine(IInteractable interactable)
        {
            _interacting = true;

            IEnumerator interaction = interactable.OnInteraction(new InteractionData(this, _input));
            if (interaction != null)
            {
                while (interaction.MoveNext())
                {
                    yield return interaction.Current;
                }
            }

            EndInteraction();
            yield return null;
        }
        
        #endregion

        #region Controls

        private void UpdateMovement()
        {
            if (!_mainCamera) _mainCamera = Camera.main;

            _currentMovement = math.distance(_oldPosition, _rigidbody.position) / Time.fixedDeltaTime;
            
            float3 movement = new float3(_movementDirection.x, 0, _movementDirection.y);

            float3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0f;
            quaternion cameraRotation = quaternion.LookRotation(cameraForward, new float3(0, 1, 0));
            movement = math.mul(cameraRotation, movement);
            
            if (CanMove && math.lengthsq(_movementDirection) > 0f)
            {
                transform.rotation = math.slerp(
                    transform.rotation,
                    quaternion.LookRotation(movement, new float3(0, 1, 0)),
                    10.0f * Time.deltaTime * RotationSpeedFactor);
            }

            _normalizedMovement = math.normalizesafe(movement);
            movement *= math.clamp(math.dot(transform.forward, _normalizedMovement), 0, 1) * movementSpeed * MovementSpeedFactor;

            _oldPosition = _rigidbody.position;
            _rigidbody.MovePosition(_rigidbody.position + (Vector3)movement * Time.fixedDeltaTime);
        }
        
        #endregion

        #region SetterGetter

        public bool GetControllable()
        {
            return CanMove;
        }

        public void SetControllable(bool value)
        {
            CanMove = value;
        }

        #endregion

        #region Helper

        public void DestroyInteractableAndRemoveFromReach(IInteractable interactable)
        {
            RemoveFromReach(interactable);
            Destroy(interactable.GameObject);
        }

        public void RemoveFromReach(IInteractable interactable)
        {
            if (_currentInteractionTarget == interactable)
            {
                _currentInteractionTarget = null;
            }
            
            if (_interactablesInReach.ContainsKey(interactable.GameObject))
            {
                _interactablesInReach.Remove(interactable.GameObject);
            }

            if (_nextInteractionTarget == interactable) _nextInteractionTarget = null;
        }

        private static IInteractable GetNearestInteractable(PlayerController controller, Transform transform, Dictionary<GameObject, IInteractable> interactables)
        {
            if (interactables.Count != 1)
            {
                float3 position = transform.position;
                float minDistance = float.MaxValue;
                IInteractable minDistanceInteractable = null;

                foreach (KeyValuePair<GameObject, IInteractable> prop in interactables)
                {
                    if (!prop.Value.IsInteractableInContext(controller)) continue;
                    
                    float distance = math.distance(position, prop.Value.GameObject.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistanceInteractable = prop.Value;
                    }
                }

                return minDistanceInteractable;
            }

            IInteractable interactable = interactables.ElementAt(0).Value;
            return interactable.IsInteractableInContext(controller) ? interactable : null;
        }
        
        public void DiscardHeldItem()
        {
            if (_heldItem)
            {
                Item item = _heldItem;
                DropHeldItem();
                RemoveFromReach(item);
                print($"Discarded held item {item.name}");
                Destroy(item.gameObject);
            }
        }

        public IEnumerator InstantiateAndEquipItem(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<Item>(out Item prop))
            {
                GameObject newInstance = Instantiate(gameObject);
                yield return null;
                PickUpItem(newInstance.GetComponent<Item>(), true, true);
            }
            else
            {
                Debug.LogWarning($"The object '{gameObject.name}' you are trying to instantiate doesnt have a prop component!");
            }
        }

        #endregion

        #region Animation

        public void SetAnimatorValue<T>(string name, T value) where T : struct
        {
            Type typeOfT = typeof(T);

            if (typeOfT == typeof(float))
            {
                animator.SetFloat(name, (float) Convert.ChangeType(value, typeof(float)));
            }
            else if (typeOfT == typeof(bool))
            {
                bool convertedValue = (bool) Convert.ChangeType(value, typeof(bool));
                AnimatorControllerParameter param = animator.parameters.First(parameter => parameter.name == name);
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(name, convertedValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (convertedValue)
                            animator.SetTrigger(name);
                        else
                            animator.ResetTrigger(name);
                        break;
                }
            }
            else if (typeOfT == typeof(int))
            {
                animator.SetInteger(name, (int) Convert.ChangeType(value, typeof(int)));
            }
        }

        private void UpdateAnimator()
        {
            if (!animator)
            {
                return;
            }

            float currMoveSpeed = math.length(_currentMovement);
            _animationMoveSpeed.Update(currMoveSpeed, 0.1f);
            animator.SetFloat("Velocity", _animationMoveSpeed);
        }

        private void RotateCharacterTowards(float angle, bool counterRotateController = true)
        {
            float targetAngleOffset =  _character.localRotation.eulerAngles.y - angle;
            _character.localRotation *= Quaternion.Euler(0, targetAngleOffset, 0);
            transform.rotation  *= Quaternion.Euler(0, targetAngleOffset, 0);
        }

        #endregion
    }
}