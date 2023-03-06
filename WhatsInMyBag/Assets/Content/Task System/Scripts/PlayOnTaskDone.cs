using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

namespace Content.Task_System
{
	public class PlayOnTaskDone : MonoBehaviour
	{
		public PlayableDirector timeline;
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
				timeline.Play();
			}
		}
	}
}