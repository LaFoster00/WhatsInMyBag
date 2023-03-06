using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;

namespace Common.Wwise.Bolt
{
    public class RaiseWwiseEvent : Unit
    {
        [DoNotSerialize] public ControlInput Input;
        [DoNotSerialize] public ControlOutput Output;
        
        [DoNotSerialize] public ValueInput GameObject;
        [Serialize] [Inspectable, UnitHeaderInspectable] public AK.Wwise.Event Event = null;

        protected override void Definition()
        {
            Input = ControlInput("Input", Enter);
            Output = ControlOutput("Output");

            GameObject = ValueInput<GameObject>("GameObject");
            
            Requirement(GameObject, Input);
        }

        private ControlOutput Enter(Flow flow)
        {
            Event.Post(flow.GetValue<GameObject>(GameObject));
            return Output;
        }
    }
    
    public class SetWwiseSwitch : Unit
    {
        [DoNotSerialize] public ControlInput Input;
        [DoNotSerialize] public ControlOutput Output;
        
        [DoNotSerialize] public ValueInput GameObject;
        [Serialize] [Inspectable, UnitHeaderInspectable] public AK.Wwise.Switch Switch;

        protected override void Definition()
        {
            Input = ControlInput("Input", Enter);
            Output = ControlOutput("Output");

            GameObject = ValueInput<GameObject>("GameObject");

            Requirement(GameObject, Input);
        }

        private ControlOutput Enter(Flow flow)
        {
            Switch.SetValue(flow.GetValue<GameObject>(GameObject));
            return Output;
        }
    }
}
