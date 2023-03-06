using System;
using UnityEngine;

namespace Interactables.Items.Crafting
{

    [CreateAssetMenu(fileName = "New Recipe", menuName = "Interactable/CraftingRecipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public ItemDescription item1;
        public ItemDescription item2;
        public ItemDescription product;

        public ItemDescription[] Items
        {
            get => new[] { item1, item2 };
        }

        public float time = 1;
    }
}