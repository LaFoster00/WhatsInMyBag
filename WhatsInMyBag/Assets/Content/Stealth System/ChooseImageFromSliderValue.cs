using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Stealth_System
{
	[ExecuteAlways]
	public class ChooseImageFromSliderValue : MonoBehaviour
	{
		public List<Sprite> images           = new List<Sprite>();
		public List<float>  valuesToChangeAt = new List<float>();

		public Slider slider;
		public Image  imageToAssign;

		private void Start()
		{
			Debug.Assert(valuesToChangeAt.Count + 1 == images.Count,
			             "Must have one more image as numbers to change at");
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying && (images.Count == 0 || valuesToChangeAt.Count == 0))
				return;
			
			float sliderValue = slider.value;
			int indexToChoose = 0;

			for (int i = 0; i < valuesToChangeAt.Count; i++)
			{
				if (valuesToChangeAt[i] < sliderValue)
				{
					indexToChoose++;
				}
			}

			imageToAssign.sprite = images[indexToChoose];
		}
	}
}