using GameEvents;
using UnityEngine;

namespace Content.Task_System
{
	public class SetActiveOnTaskDone : MonoBehaviour
	{
		public bool setActive = true;
		public GameObject obj;
		public Task onTask;

		private void OnEnable()
		{
			GameEventManager.AddListener<TaskFinishedEvent>(OnTask);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<TaskFinishedEvent>(OnTask);
		}

		private void OnTask(TaskFinishedEvent e)
		{
			if (e.FinishedTask == onTask)
			{
				obj.SetActive(setActive);
			}
		}
	}
}