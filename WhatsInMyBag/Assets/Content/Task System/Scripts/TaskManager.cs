using System;
using GameEvents;
using UnityEngine;

namespace Content.Task_System
{
	public class TaskManager : MonoBehaviour
	{
		public Task current => _currrent;

		[SerializeField] private Task _currrent;

		public static TaskManager Instance;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private void OnEnable()
		{
			GameEventManager.AddListener<TaskEvent>(OnTaskAction);
			GameEventManager.AddListener<NewTaskEvent>(OnTaskToChange);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<TaskEvent>(OnTaskAction);
			GameEventManager.RemoveListener<NewTaskEvent>(OnTaskToChange);
		}

		public void OnTaskAction(TaskEvent e)
		{
			if (e.BelongingTask == current)
			{
				Debug.Log($"Finished task: {current.text}");
				GameEventManager.Raise(new TaskFinishedEvent {FinishedTask = current});

				_currrent = current.nextTask;
			}
		}

		private void OnTaskToChange(NewTaskEvent e)
		{
			_currrent = e.Task;
		}

		public void SetTask(Task taskToSet)
		{
			_currrent = taskToSet;
		}
	}
}