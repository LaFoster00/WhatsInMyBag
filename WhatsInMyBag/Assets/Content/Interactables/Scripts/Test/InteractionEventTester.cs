using System;
using Bolt;
using Interactables.Bolt;
using Ludiq;
using UnityEngine;

public class InteractionEventTester : MonoBehaviour
{
    public FlowMacro usedMacro;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GraphReference interactionMacroRef = GraphReference.New(usedMacro, true);
        interactionMacroRef.TriggerEventHandler(hook => hook.name == "OnItemInteract", new ItemInteractionEventArgs(null, null, null), _ => false, true);
    }
}
