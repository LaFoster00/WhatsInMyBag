using System;
using System.Collections;
using System.Linq;
using GameEvents;
using Interactables.Items.Crafting;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;
using USCSL;
using Random = UnityEngine.Random;

namespace Interactables.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour, IInteractable
    {
        #region PROPERTIES

        [SerializeField] private bool isInteractable = true;

        public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
        public bool IsHighlighted { get => gameObject.layer == LayerMask.NameToLayer("Outline"); }
        public GameObject GameObject { get => gameObject; }
        
        public ItemDescription description;

        private Rigidbody _rigidbody;
        private bool _recipeAvailable;
        private int _fallbackLayer;
        private int _defaultLayer;
        private bool _resetLayerToDefaultOnCollision;
        private bool _itemThrown;

        private Coroutine _offsetUpdater;

        private static int _noPlayerCollisionLayer;
        
        #endregion

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            gameObject.tag = "Interactable";
            _fallbackLayer = this.gameObject.layer;
            if (!description)
            {
                Debug.LogWarning($"This item does have a description assigned: {name}");
                gameObject.SetActive(false);
            }

            _defaultLayer = LayerMask.NameToLayer("Interactable");
            gameObject.layer = _defaultLayer;

            if (_rigidbody && gameObject.isStatic) _rigidbody.isKinematic = true;
            _noPlayerCollisionLayer = LayerMask.NameToLayer("No Player Collision");
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

        public static CraftingRecipe GetViableCraftingRecipe(Item a, Item b)
        {
            foreach (var recipe in a.description.recipes)
            {
                if (recipe && recipe.Items.Contains(a.description) && recipe.Items.Contains(b.description))
                {
                    return recipe;
                }
            }
            
            return null;
        }
        
        private void OnItemPickedUp(ItemPickedUpEvent @event)
        {
            CraftingRecipe recipe = GetViableCraftingRecipe(this, @event.Item);
            if (recipe)
            {
                _recipeAvailable = true;
                GameEventManager.Raise(new RecipeAvailableEvent(this, @event.Item, recipe));
            }
        }

        private void OnItemDropped(ItemDroppedEvent @event)
        {
            _recipeAvailable = false;
        }

        public bool IsInteractableInContext(PlayerController controller)
        {
            return isInteractable && (!controller.HeldItem && !controller.HeldBody && !controller.HeldProp || _recipeAvailable);
        }

        public IEnumerator OnInteraction(InteractionData data)
        {
            if (data.PlayerController.HeldItem && data.PlayerController.HeldItem != this)
            {
                CraftingRecipe recipe = GetViableCraftingRecipe(this, data.PlayerController.HeldItem);

                if (recipe)
                {
                    return CombineItems(data, recipe, this);
                }
            }
            else if (!data.PlayerController.HeldItem && !data.PlayerController.HeldProp && !data.PlayerController.HeldBody)
            {
                data.PlayerController.PickUpItem(this);
                _offsetUpdater = StartCoroutine(UpdateOffset(data));
            }
            else
            {
                if (_offsetUpdater != null)
                {
                    StopCoroutine(_offsetUpdater);
                    _offsetUpdater = null;
                }
                return DropItem(data);
            }

            return null;
        }

        private IEnumerator UpdateOffset(InteractionData data)
        {
            while (true)
            {
                transform.localPosition = description.localGrabOffset /
                                          data.PlayerController.MahleeCharacter.transform.localScale;
                transform.localRotation = quaternion.Euler(math.radians(description.localRotationOffset));
                yield return null;
            }
        }
        
        private IEnumerator DropItem(InteractionData data)
        {
            float heldTime = 0;
            while (!data.Input.InteractUp)
            {
                heldTime += Time.deltaTime;
                if (heldTime > 0.2f)
                {
                    data.PlayerController.SetAnimatorValue("Throw", false);
                    data.PlayerController.SetAnimatorValue("IsAiming", true);
                }
                yield return null;
            }
            data.PlayerController.SetAnimatorValue("Throw", true);
            data.PlayerController.SetAnimatorValue("IsAiming", false);
            
            yield return new WaitForSeconds(0.2f);
            
            _resetLayerToDefaultOnCollision = true;
            gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("No Player Collision"));
            data.PlayerController.DropHeldItem();
            data.PlayerController.RemoveFromReach(this);
            math.clamp(heldTime, 0, 2);
            if (heldTime > 0.5f) data.PlayerController.throwEvent.Post(gameObject);
            _rigidbody.AddForce((data.PlayerController.transform.forward + new Vector3(0, 0.2f, 0)) * math.clamp(heldTime * 10, 0, 10), ForceMode.Impulse);
            _rigidbody.AddTorque(new float3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40)));
            _itemThrown = true;
        }

        private static IEnumerator CombineItems(InteractionData data, CraftingRecipe recipe, Item target)
        {
            GameEventManager.Raise(new CraftingHappenedEvent(target));

            print($"Start Crafting {recipe.product.itemName} now!");
            yield return new WaitForSeconds(recipe.time);
            
            data.PlayerController.DiscardHeldItem();
            data.PlayerController.DestroyInteractableAndRemoveFromReach(target);
            
            print($"Crafted Item {recipe.product.itemName}");
            yield return data.PlayerController.InstantiateAndEquipItem(recipe.product.prefab);
        }
        
        private IEnumerator DelayedDestroy()
        {
            yield return null;
            Destroy(gameObject);
        }
        
        public void Highlight(PlayerController controller)
        {
            if (gameObject.layer == LayerMask.NameToLayer("No Player Collision")) return;

            _fallbackLayer = gameObject.layer;
            gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("Outline"));
        }

        public void RemoveHighlight(PlayerController controller)
        {
            if (gameObject.layer == LayerMask.NameToLayer("No Player Collision")) return;
            gameObject.ChangeLayerRecursive(_fallbackLayer);
        }

        public void ChangeFallbackLayer(string newFallbackLayer)
        {
            _fallbackLayer = LayerMask.NameToLayer(newFallbackLayer);
        }
        
        public void SetKinematic(bool value, bool detectCollisions = true)
        {
            _rigidbody.isKinematic = value;
            _rigidbody.detectCollisions = detectCollisions;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (_resetLayerToDefaultOnCollision)
            {
                _resetLayerToDefaultOnCollision = false;
                gameObject.ChangeLayerRecursive(_defaultLayer);
            }
            if (_itemThrown && (description.emitEventOnImpact || description.emitAudioOnImpact))
            {
                if (collision.collider.CompareTag("Player")) return;
                
                _itemThrown = false;
                float3 relativeVelocity = collision.relativeVelocity;
                float relativeVelocityMagnitude = math.length(relativeVelocity);
                if (relativeVelocityMagnitude >= description.impactEventSpeedThreshold)
                {
                    //Debug.Log(name + relativeVelocityMagnitude);
                    if (description.impactAudioEvent.IsValid()) description.impactAudioEvent.Post(gameObject);
                    GameEventManager.Raise(new InteractableImpactEvent(this, description.eventIdentifier));
                }
            }
        }
    }
}