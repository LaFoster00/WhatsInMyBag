using System;
using Interactables.Items;
using Interactables.Items.Crafting;
using Interactables.Props;
using Player.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameEvents
{  

  //---------------------------------------------------------------------------------------------------
  // sample events without additional arguments

  public class GameEvent_Click : GameEvent {}
  public class GameEvent_CancelAction : GameEvent {}
  public class GameEvent_FinishLevel : GameEvent {}

  //---------------------------------------------------------------------------------------------------

  // sample events with additional arguments. Make sure these implement Validate()

  [Serializable]
  /// <summary>
  /// A list of the possible simplified Game Engine base events
  /// </summary>
  public enum SimpleEventType
  {
    LevelStart,
    LevelComplete,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    Respawn,
    StarPicked
  }
  
  public class SimpleEvent : GameEvent
  {
    public readonly SimpleEventType eventType;
    public SimpleEvent(SimpleEventType t)
    {
      eventType = t;
    }
    
    public override bool isValid() {
      return (eventType != null);
    }
  }

  public enum GuiEventType
  {
    Enable_DebugInfo,
    Disable_DebugInfo,
  }

  public class GuiEvent : GameEvent
  {
    public GuiEventType Type;

    public GuiEvent(GuiEventType type)
    {
      Type = type;
    }
  }

  public class ITMachineBrokeEvent : GameEvent
  {
	  public readonly Prop Machine;

    public ITMachineBrokeEvent(Prop machine)
    {
      Machine = machine;
    }
  }
  
  public class EnemySeesBodyEvent : GameEvent
  {
	  public GameObject enemy;

	  public EnemySeesBodyEvent(GameObject enemy)
	  {
		  this.enemy = enemy;
	  }
  }

  public class PlayerReachGoalEvent : GameEvent
  {
    public readonly Transform target;
    public readonly Vector3 direction;

    public PlayerReachGoalEvent(Transform t, Vector3 d) {
      target = t;
      direction = d;
    }
    
    public override bool isValid ()
    {
      return target != null && direction.magnitude > 0;
    }
  }

  public class PropInteractionAvailableEvent : GameEvent
  {
    public Prop Target;
    public PlayerController Controller;

    /*
    if (Target is Prop propTarget)
    {
      bool IsMoveable = propTarget.Description.behaviour == PropInteraction.Moveable;
    }
    else if (Target is Item itemTarget)
    {
      Debug.Log(itemTarget.description.itemName);
      GameObject.Instantiate(itemTarget.description.prefab);
    }
    */
    
    public PropInteractionAvailableEvent(Prop target, PlayerController controller)
    {
      Target = target;
      Controller = controller;
    }
  }

  public class ItemPickedUpEvent : GameEvent
  {
    public Item Item;
    public PlayerController Controller;

    public ItemPickedUpEvent(Item item, PlayerController controller)
    {
      Item = item;
      Controller = controller;
    }
  }
  
  public class ItemDroppedEvent : GameEvent
  {
    public Item Item;
    public PlayerController Controller;

    public ItemDroppedEvent(Item item, PlayerController controller)
    {
      Item = item;
      Controller = controller;
    }
  }

  public class RecipeAvailableEvent : GameEvent
  {
    public Item Target;
    public Item HeldItem;
    public CraftingRecipe Recipe;

    public RecipeAvailableEvent(Item target, Item heldItem, CraftingRecipe recipe)
    {
      Target = target;
      HeldItem = heldItem;
      Recipe = recipe;
    }
  }

  public class InteractableImpactEvent : GameEvent
  {
    public IInteractable Interactable;
    public string Payload;

    public InteractableImpactEvent(IInteractable interactable, string payload)
    {
      Interactable = interactable;
      Payload = payload;
    }
  }

  public class CraftingHappenedEvent : GameEvent
  {
    public Item ItemInteractedWith;

    public CraftingHappenedEvent(Item itemInteractedWith)
    {
      ItemInteractedWith = itemInteractedWith;
    }
    
  }
  
  public class InteractionHappenedEvent : GameEvent
  {
    public Prop PropInteractedWith;

    public InteractionHappenedEvent(Prop propInteractedWith)
    {
      PropInteractedWith = propInteractedWith;
    }
    
  }

  public class EnteringViableInteractableRangeEvent : GameEvent
  {
    public GameObject HighlightedInteractable;

    public EnteringViableInteractableRangeEvent(GameObject highlightedInteractable)
    {
      HighlightedInteractable = highlightedInteractable;
    }
  }

  public class ExitingViableInteractableRangeEvent : GameEvent
  {
    public GameObject HighlightedInteractable;

    public ExitingViableInteractableRangeEvent(GameObject highlightedInteractable)
    {
      HighlightedInteractable = highlightedInteractable;
    }
  }

  public class InputDeviceChangedEvent : GameEvent
  {
    public string Scheme;

    public InputDeviceChangedEvent(string scheme)
    {
      Scheme = scheme;
    }
  }

  public class CheckpointActivatedEvent : GameEvent
  {
    public GameObject Checkpoint;

    public CheckpointActivatedEvent(GameObject checkpoint)
    {
      Checkpoint = checkpoint;
    }
  }
}