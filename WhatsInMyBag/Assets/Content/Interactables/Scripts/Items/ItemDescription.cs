using System.Collections.Generic;
using Interactables.Items.Crafting;
using Unity.Mathematics;
using UnityEngine;

namespace Interactables.Items
{
    [CreateAssetMenu(fileName = "NewItemDescription", menuName = "Interactable/ItemDescription")]
    public class ItemDescription : ScriptableObject
    {
        [Header("Grabing")]
        public float3 localGrabOffset = new float3(0, 0.65f, 0f);
        public float3 localRotationOffset = new float3(0, -90, 0);
        
        [Header("Item")] public string itemName;
        public GameObject prefab;
        public Sprite thumbnail;

        [Header("Throwing")] 
        public bool emitEventOnImpact = false;
        public string eventIdentifier = "ItemImpact";
        public float impactEventSpeedThreshold = 5;
        public bool emitAudioOnImpact = true;
        public AK.Wwise.Event impactAudioEvent;

        [Header("Crafting")] public List<CraftingRecipe> recipes = new List<CraftingRecipe>();
    }
}