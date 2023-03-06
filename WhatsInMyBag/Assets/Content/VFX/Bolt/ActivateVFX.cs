using Bolt;
using Interactables.Props;
using Ludiq;
using Props.Description.Data;

[UnitCategory("VFX")]
public class ActivateExplosion : Unit
{
    [DoNotSerialize] public ControlInput Input;
    [DoNotSerialize] public ControlOutput Output;
    [DoNotSerialize] public ValueInput Prop;
    [DoNotSerialize] public ValueOutput Explosion;
    [DoNotSerialize, PortLabelHidden] public ValueOutput PropOut;
    
    protected override void Definition()
    {
        Input = ControlInput("", Enter);
        Output = ControlOutput("");
        Prop = ValueInput<Prop>("Prop");
        PropOut = ValueOutput<Prop>("Out");
        Explosion = ValueOutput<ExplosionVFX>("ExplosionVFX");
    }

    private ControlOutput Enter(Flow flow)
    {
        Prop prop = flow.GetValue<Prop>(Prop);
        flow.SetValue(PropOut, prop);
        ExplosionVFX explosionVFX = flow.GetValue<IInteractable>(Prop).GameObject.GetComponentInChildren<ExplosionVFX>();
        if (explosionVFX)
        {
            explosionVFX.Explode();
            if (prop.GetCustomDataGeneric(out CD_Destructable data))
            {
                data.ExplosionVFX = explosionVFX;
                data.Damaged = true;
            }
        }
        flow.SetValue(Explosion, explosionVFX);
        return Output;
    }
}

[UnitCategory("VFX")]
public class ActivatePaperStorm : Unit
{
    [DoNotSerialize] public ControlInput Input;
    [DoNotSerialize] public ControlOutput Output;
    [DoNotSerialize] public ValueInput Prop;
    [DoNotSerialize] public ValueOutput Paperstorm;
    [DoNotSerialize, PortLabelHidden] public ValueOutput PropOut;
    
    protected override void Definition()
    {
        Input = ControlInput("", Enter);
        Output = ControlOutput("");
        Prop = ValueInput<Prop>("Prop");
        PropOut = ValueOutput<Prop>("Out");
        Paperstorm = ValueOutput<PrinterPaperstorm>("PaperstormVFX");
    }

    private ControlOutput Enter(Flow flow)
    {
        Prop prop = flow.GetValue<Prop>(Prop);
        flow.SetValue(PropOut, prop);
        PrinterPaperstorm paperstromEffect = flow.GetValue<IInteractable>(Prop).GameObject.GetComponentInChildren<PrinterPaperstorm>();
        if (paperstromEffect)
        {
            paperstromEffect.PlayEffect();
            if (prop.GetCustomDataGeneric(out CD_Destructable data))
            {
                data.PaperstormVFX = paperstromEffect;
            }
        }
        flow.SetValue(Paperstorm, paperstromEffect);
        return Output;
    }
}