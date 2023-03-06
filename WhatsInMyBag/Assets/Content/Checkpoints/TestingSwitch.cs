using System;
using Content.Onboarding.Scripts;
using Content.Task_System;
using Ludiq;
using UnityEngine;

namespace Content.Checkpoints
{
	[ExecuteAlways]
	public class TestingSwitch : MonoBehaviour
	{
		public bool isFinalGameplay = true;
		public bool allowLoad = false;

		[Header("References")] public GameObject questObject;
		public GameObject blocker;
		public GameObject introTimeline;

		public CheckpointsManager checkpoints;
		public SaveDoneIntros introSaver;
		public TaskManager taskManager;
		public GameObject debugUI;

		private void Start()
		{
			if (!Application.isEditor)
			{
				isFinalGameplay = true;
				allowLoad = true;
			}
			
			if (Application.isPlaying)
			{
				SetActiveObjects();

				if (!isFinalGameplay)
				{
					taskManager.SetTask(null);
				}
			}
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				SetActiveObjects();
			}
		}

		private void SetActiveObjects()
		{
			questObject.SetActive(isFinalGameplay);
			blocker.SetActive(isFinalGameplay);
			introTimeline.SetActive(isFinalGameplay);
			checkpoints.doLoad = allowLoad || isFinalGameplay;
			introSaver.doLoad = allowLoad || isFinalGameplay;
			debugUI.SetActive(!isFinalGameplay);
		}
	}
}