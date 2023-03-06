using System;
using UnityEngine;
using UnityEngine.Video;

namespace Content.Onboarding.Scripts
{
	public class SwitchSceneOnVideoFinished : MonoBehaviour
	{
		public VideoPlayer player;
		public int sceneToLoad;

		private bool _wasPlaying;

		private void LateUpdate()
		{
			if (!player.isPlaying)
			{
				if (_wasPlaying)
					LevelManager.Instance.SwitchScene(sceneToLoad);
			}
			else
			{
				_wasPlaying = true;
			}
		}
	}
}