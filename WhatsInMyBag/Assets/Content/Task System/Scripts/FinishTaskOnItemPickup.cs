using GameEvents;
using UnityEngine;

namespace Content.Task_System
{
	public class FinishTaskOnItemPickup : MonoBehaviour
	{
		public string itemNameToPickup;
		public Task taskToFinish;

		private void OnEnable()
		{
			GameEventManager.AddListener<TaskFinishedEvent>(OnTaskFinished);
			GameEventManager.AddListener<ItemPickedUpEvent>(OnItemPickup);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<TaskFinishedEvent>(OnTaskFinished);
			GameEventManager.RemoveListener<ItemPickedUpEvent>(OnItemPickup);
		}

		private void OnItemPickup(ItemPickedUpEvent e)
		{
			if (e.Item.description.itemName == itemNameToPickup)
				GameEventManager.Raise(new TaskEvent {BelongingTask = taskToFinish});
		}

		void OnTaskFinished(TaskFinishedEvent e)
		{
			if (e.FinishedTask == taskToFinish)
			{
				Destroy(gameObject);
			}
		}
	}
}