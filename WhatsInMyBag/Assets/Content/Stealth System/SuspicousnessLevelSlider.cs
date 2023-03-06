using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Stealth_System
{
	[RequireComponent(typeof(Slider))]
	public class SuspicousnessLevelSlider : MonoBehaviour
	{
		private Slider _slider;

		private void Start()
		{
			_slider = GetComponent<Slider>();
		}

		private void LateUpdate()
		{
			if (PlayerAlertMeter.Instance)
				_slider.value = PlayerAlertMeter.Instance.Alertness;
		}
	}
}