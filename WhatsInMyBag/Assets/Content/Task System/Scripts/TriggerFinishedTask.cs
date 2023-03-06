using System;
using GameEvents;
using UnityEngine;

namespace Content.Task_System
{
	public class TriggerFinishedTask : MonoBehaviour
	{
		public Task taskToFinish;

		private void OnEnable()
		{
			GameEventManager.AddListener<TaskFinishedEvent>(OnTaskFinished);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<TaskFinishedEvent>(OnTaskFinished);
		}

		private void OnTriggerEnter(Collider other)
		{
			GameEventManager.Raise(new TaskEvent{BelongingTask = taskToFinish});
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