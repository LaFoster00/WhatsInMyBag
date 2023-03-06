using Content.Onboarding.Scripts;
using GameEvents;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Content.Task_System
{
	public class LoadSceneOnFinishedTask : MonoBehaviour
	{
		public int sceneToLoad;
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
			if (e.FinishedTask == onTask && SaveDoneIntros.Instance.CanPlayIntro(name))
			{
				LevelManager.Instance.SwitchScene(sceneToLoad);
				
				GameEventManager.Raise(new PlayedIntroEvent() {name = name});
			}
		}
	}
}