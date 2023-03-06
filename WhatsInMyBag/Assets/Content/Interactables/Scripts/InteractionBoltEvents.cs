using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bolt;
using Interactables.Items;
using Interactables.Props;
using Ludiq;
using Player.Controller;

namespace Interactables.Bolt
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct PropInteractionEventArgs
    {
        public Prop prop;
        public PlayerController playerController;

        public PropInteractionEventArgs(PlayerController playerController, Prop prop)
        {
            this.playerController = playerController;
            this.prop = prop;
        }
    }

    [UnitCategory("Interaction")]
    public class OnPropInteraction : EventUnit<PropInteractionEventArgs>
    {
        //auto execute StartListening, lead to assign arguments being called and trigger() working natively 
        protected override bool register => true;

        private Dictionary<string, ValueOutput> _reflectedAttributeFields = new Dictionary<string, ValueOutput>();

        protected override void Definition()
        {
            base.Definition(); //important: sets isrootnode and exposes output "trigger"
            
            //clear, otherwise old ports wont be released correctly before being added to unit again
            _reflectedAttributeFields.Clear();

            foreach (var field in typeof(PropInteractionEventArgs).GetFields())
            {
                _reflectedAttributeFields.Add(field.Name, ValueOutput(field.FieldType, field.Name));
            }
        }

        protected override void AssignArguments(Flow flow, PropInteractionEventArgs args)
        {
            foreach (var field in typeof(PropInteractionEventArgs).GetFields())
            {
                flow.SetValue(_reflectedAttributeFields[field.Name], field.GetValue(args));
            }
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("OnPropInteract");
        }
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ItemInteractionEventArgs
    {
        public PlayerController playerController;
        public Item item;
        public Prop targetObject;

        public ItemInteractionEventArgs(
            PlayerController playerController,
            Item item,
            Prop targetObject)
        {
            this.playerController = playerController;
            this.item = item;
            this.targetObject = targetObject;
        }
    }
    
    [UnitCategory("Interaction")]
    public class OnItemInteraction : EventUnit<ItemInteractionEventArgs>
    {
        //auto execute StartListening, lead to assign arguments being called and trigger() working natively 
        protected override bool register => true;

        private Dictionary<string, ValueOutput> _reflectedAttributeFields = new Dictionary<string, ValueOutput>();

        protected override void Definition()
        {
            base.Definition(); //important: sets isrootnode and exposes output "trigger"
            
            //clear, otherwise old ports wont be released correctly before being added to unit again
            _reflectedAttributeFields.Clear();

            foreach (var field in typeof(ItemInteractionEventArgs).GetFields())
            {
                _reflectedAttributeFields.Add(field.Name, ValueOutput(field.FieldType, field.Name));
            }
        }

        protected override void AssignArguments(Flow flow, ItemInteractionEventArgs args)
        {
            foreach (var field in typeof(ItemInteractionEventArgs).GetFields())
            {
                flow.SetValue(_reflectedAttributeFields[field.Name], field.GetValue(args));
            }
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("OnItemInteract");
        }
    }

}