using System;
using System.Collections;
using Content.Task_System;
using GameEvents;
using Player.Controller;
using UnityEngine;
using USCSL;

public class DeadBodyDisposal : MonoBehaviour, IInteractable
{
	[SerializeField] private string outroSceneName = "Outro";
	[SerializeField] private Task mainTask;

	private bool _isInteractable = true;

	public bool IsInteractable
	{
		get => _isInteractable;
		set => _isInteractable = value;
	}

	private bool _isHighlighted;

	public bool IsHighlighted
	{
		get => _isHighlighted;
	}

	public GameObject GameObject
	{
		get => gameObject;
	}

	private void Awake()
	{
		gameObject.tag = "Interactable";
	}

	public bool IsInteractableInContext(PlayerController controller)
	{
		return controller.HeldBody;
	}

	public IEnumerator OnInteraction(InteractionData data)
	{
		if (data.PlayerController.HeldBody)
		{
			// finished task
			GameEventManager.Raise(new TaskEvent {BelongingTask = mainTask});
			
			LevelManager.Instance.SwitchScene(outroSceneName);
		}

		return null;
	}

	private static void ChangeLayerRecursive(GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			ChangeLayerRecursive(gameObject.transform.GetChild(i).gameObject, layer);
		}
	}

	public void Highlight(PlayerController controller)
	{
		gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("Outline"));
		_isHighlighted = true;
	}

	public void RemoveHighlight(PlayerController controller)
	{
		gameObject.ChangeLayerRecursive(LayerMask.NameToLayer("Default"));
		_isHighlighted = false;
	}
}