using System;
using System.Collections;
using Content.Stealth_System;
using Ludiq;
using UnityEngine;

namespace Content.Game_Over.Scripts
{
	public class GameOverChecker : MonoBehaviour
	{
		[Tooltip("Amount of time in seconds the player is allowed to be seen without getting an INSTANT game over")]
		public float maximumAllowedTimeOnFound = 0.1f;

		public float accuracy = 0.05f;

		[SerializeField] private MenuManager menuManager;
		[SerializeField] public Submenu gameOverScreen;

		private float _lastUnseenTime = 0;

		public static GameOverChecker Instance   { get; private set; }
		public        bool            IsGameOver { get; private set; }

		private void Awake()
		{
			Time.timeScale = 1;
		}

		private void Start()
		{
			Instance = this;
		}

		private void Update()
		{
			if (Math.Abs(PlayerAlertMeter.Instance.Alertness - PlayerAlertMeter.Instance.MaximumAlertness) > accuracy)
			{
				_lastUnseenTime = Time.time;
			}
			else
			{
				float currTime = Time.time;

				if (currTime - _lastUnseenTime > maximumAllowedTimeOnFound)
				{
					DoGameOver();
				}
			}
		}

		private void DoGameOver()
		{
			AkSoundEngine.SetState("Normal_or_Busted", "Busted");
			IsGameOver = true;
			menuManager.SwitchToMenu(gameOverScreen);
			Time.timeScale = 0;
		}

		public void DoRetry()
		{
			AkSoundEngine.SetState("Normal_or_Busted", "Normal");
			print("Reloading Scene");
			LevelManager.Instance.ResetScene();
		}
	}
}