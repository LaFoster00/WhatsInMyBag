using System;
using Bolt;
using Ludiq;

[UnitCategory("Control")]
public class CompareType : Unit
{
    /// <summary>
    /// The entry point for the sequence.
    /// </summary>
    [DoNotSerialize]
    [PortLabelHidden]
    public ControlInput enter { get; private set; }

    [DoNotSerialize] [Inspectable, InspectorLabel("Type"), UnitHeaderInspectable]
    public string type;

    [DoNotSerialize] [PortLabelHidden] public ValueInput target;

    [DoNotSerialize] [PortLabel("Equal")] public ControlOutput equalOut;
    [DoNotSerialize] [PortLabel("NotEqualOut")] public ControlOutput notEqualOut;

    protected override void Definition()
    {
        enter = ControlInput("enter", Enter);
        equalOut = ControlOutput("EqualOut");
        notEqualOut = ControlOutput("NotEqualOut");
        target = ValueInput(typeof(Type), "target");
    }

    private ControlOutput Enter(Flow flow)
    {
        if (flow.GetValue<Type>(target).Name == type) 
        {
            return equalOut;
        }

        return notEqualOut;
    }
}
