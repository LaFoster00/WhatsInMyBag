using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Props;
using UnityEngine;
using UnityEngine.Events;

namespace Content.NPC.Scripts
{
    [Serializable]
    public enum AlertionType
    {
        General,
        CoffeeMachine,
        MicrowaveFood,
        MicrowaveExplosion,
        Explosion,
        Dirt,
        Printer,
        Food,
        Fridge
    }

    public class AlertionInfo
    {
	    public AlertionInfo(List<AlertionType> alertTypes, GameObject alertionObject, PropDescription prop, float alertionDuration)
	    {
		    this.alertTypes = alertTypes;
		    this.alertionObject = alertionObject;
		    this.prop = prop;
		    this.alertionDuration = alertionDuration;
	    }

	    public readonly List<AlertionType> alertTypes;

	    public readonly GameObject alertionObject;

	    /// distance from which an NPC may stand at an alertion is stored in prop description
	    public readonly PropDescription prop;
	    
	    /// <summary>
	    /// How long an NPC may be distracted by this alertion
	    /// </summary>
	    public readonly float alertionDuration;
    }
	
    public class AlertableNPC : MonoBehaviour
    {
	    [Tooltip("To which alertion this npc can react to")]
        public List<AlertionType> alertionTypesToReactTo = new List<AlertionType> { AlertionType.General };
		
        [Tooltip("Functions that wil lbe called on alertion")]
        public UnityEvent<AlertionInfo> onAlert = new UnityEvent<AlertionInfo>();

        public void Alert(AlertionInfo alertion)
        {
            onAlert.Invoke(alertion);
        }

        public bool IsAlertedOn(AlertionType type)
        {
            return alertionTypesToReactTo.Contains(type);
        }
        
        public bool IsAlertedOn(List<AlertionType> types)
        {
	        return alertionTypesToReactTo.Any(alertionType => types.Any(type => type == alertionType));
        }
    }
}