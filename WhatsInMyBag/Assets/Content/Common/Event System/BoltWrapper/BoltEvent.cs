#define USE_BOLT
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if USE_BOLT
using Bolt;
using Ludiq;

namespace GameEvents
{
    class GenericEventReceiver<T> where T : GameEvent
    {
        private GraphReference _graphReference;
        private Dictionary<string, ValueOutput> _attributeFields;
        private ControlOutput _controlOutput;

        public GenericEventReceiver(GraphReference reference, Dictionary<string, ValueOutput> attributeFields, ControlOutput controlOutput)
        {
            _graphReference = reference;
            _attributeFields = attributeFields;
            _controlOutput = controlOutput;
        }

        public void Subscribe()
        {
            GameEventManager.AddListener<T>(OnGenericEvent);
        }

        public void Unsubscribe()
        {
            GameEventManager.RemoveListener<T>(OnGenericEvent);
        }
        
        public void OnGenericEvent(T eventData)
        {
            Flow flow = Flow.New(_graphReference);
            foreach (var field in typeof(T).GetFields())
            {
                if (_attributeFields.ContainsKey(field.Name))
                {
                    flow.SetValue(_attributeFields[field.Name], field.GetValue(eventData));
                }
                
            }
            flow.Invoke(_controlOutput);
        }
    }
    
    [UnitCategory("Events")]
    public class OnEventReceive : EventUnit<EmptyEventArgs>
    {
        protected override bool register => false;
        
        [Serialize, UnitHeaderInspectable] public Type eventType = typeof(SimpleEvent);
        
        private Dictionary<string, ValueOutput> _reflectedAttributeFields = new Dictionary<string, ValueOutput>();

        private GraphReference _graphReference;

        private object _receiver;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("UnusedEvent", (object) reference.machine);
        }
        
        protected override void Definition()
        {
            base.Definition(); //important: sets isrootnode and exposes output "trigger"
            
            //clear, otherwise old ports wont be released correctly before being added to unit again
            _reflectedAttributeFields.Clear();

            if (eventType.BaseType == typeof(GameEvent))
            {
                foreach (var field in eventType.GetFields())
                {
                    _reflectedAttributeFields.Add(field.Name, ValueOutput(field.FieldType, field.Name));
                }
            }
        }
        
        public override void StartListening(GraphStack stack)
        {
            Debug.Log("Started Listening");
            base.StartListening(stack);

            _graphReference = stack.AsReference();
            if (eventType.BaseType == typeof(GameEvent))
            {
                Type receiverType = typeof(GenericEventReceiver<>).MakeGenericType(eventType);
                _receiver = Activator.CreateInstance(receiverType, _graphReference, _reflectedAttributeFields, trigger);
                _receiver.GetType().GetMethod("Subscribe").Invoke(_receiver, null);
            }
        }

        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);
            var data = stack.GetElementData<Data>(this);
            _receiver.GetType().GetMethod("Unsubscribe").Invoke(_receiver, null);
        }
    }

    class GenericEventRaiser<T> where T : GameEvent
    {
        public void RaiseEvent(T @event)
        {
            GameEventManager.Raise(@event);
        }
            
    }
    
    [UnitCategory("Events")]
    public class RaiseEvent : Unit
    {        
        [Serialize, UnitHeaderInspectable] public Type eventType = typeof(SimpleEvent);

        private ControlInput _input;

        private Dictionary<string, ValueInput> _reflectedAttributeFields = new Dictionary<string, ValueInput>();

        private ControlOutput _output;

        private Type _raiserType;
        private object _raiser;
        
        protected override void Definition()
        {
            _input = ControlInput("In", Enter);
            //clear, otherwise old ports wont be released correctly before being added to unit again
            _reflectedAttributeFields.Clear();

            if (eventType.BaseType == typeof(GameEvent))
            {
                foreach (var field in eventType.GetFields())
                {
                    _reflectedAttributeFields.Add(field.Name, ValueInput(field.FieldType, field.Name));
                    Requirement(_reflectedAttributeFields[field.Name], _input);
                }
            }

            _output = ControlOutput("Out");
            
            if (eventType.BaseType == typeof(GameEvent))
            {
                _raiserType = typeof(GenericEventRaiser<>).MakeGenericType(eventType);
                _raiser = Activator.CreateInstance(_raiserType, null);
            }
        }


        public ControlOutput Enter(Flow flow)
        {
            object[] attributes = new object[_reflectedAttributeFields.Count];
            ValueInput[] valueInputs = _reflectedAttributeFields.Values.ToArray();
            for (int i = 0; i < _reflectedAttributeFields.Count; i++)
            {
                attributes[i] = flow.GetValue(valueInputs[i]);
            }
            
            _raiserType.GetMethod("RaiseEvent").Invoke(_raiser, new []{ Activator.CreateInstance(eventType, attributes) } );
            return _output;
        }
    }
}

#endif