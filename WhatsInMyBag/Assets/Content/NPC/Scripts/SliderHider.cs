using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.NPC.Scripts
{
	public class SliderHider : MonoBehaviour
	{
		public float  minValueToShow = 0.0001f;
		public float  maxValue       = 0.99f;
		public Slider slider;


		private void LateUpdate()
		{
			if (slider.value < minValueToShow)
			{
				if (slider.gameObject.activeSelf)
				{
					slider.gameObject.SetActive(false);
				}
			} else if (slider.value > maxValue)
			{
				if (slider.gameObject.activeSelf)
				{
					slider.gameObject.SetActive(false);
				}
			}
			else if (!slider.gameObject.activeSelf)
			{
				slider.gameObject.SetActive(true);
			}
		}
	}
}