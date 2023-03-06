using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Stealth_System
{
	public class PlayerAlertMeter : MonoBehaviour
	{
		public float  lossPerSec = 0.1f;

		[SerializeField] [Tooltip("The maximum amount of alertness the player can have")]
		private float maximumAlertness = 1.0f;
		
		public static PlayerAlertMeter Instance { get; private set; }

		public float Alertness {
			get;
			private set;
		}

		public float MaximumAlertness => maximumAlertness;

		private void Start()
		{
			Instance = this;
		}

		private void LateUpdate()
		{
			Alertness -= lossPerSec * Time.deltaTime;
			Alertness = Mathf.Max(0, Alertness);
		}

		public void AddAlertness(float level)
		{
			Alertness += level;
			Alertness = Mathf.Min(maximumAlertness, Alertness);
		}
	}
}