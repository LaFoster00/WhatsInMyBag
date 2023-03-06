using System.Collections;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;

public readonly struct InteractionData
{
    public readonly PlayerController PlayerController;
    public readonly InteractionInput Input;

    public InteractionData(PlayerController controller, InteractionInput input)
    {
        PlayerController = controller;
        Input = input;
    }
}

public class InteractionInput
{
    public bool InteractDown;
    public bool Interact;
    public bool InteractUp;
    public float2 MovementDir;
}

public interface IInteractable
{
    public bool IsInteractable { get; set; }
    public bool IsHighlighted { get; }
    public GameObject GameObject { get; }
    public bool IsInteractableInContext(PlayerController controller);

    
    public IEnumerator OnInteraction(InteractionData data);

    public void Highlight(PlayerController controller);
    public void RemoveHighlight(PlayerController controller);
}
