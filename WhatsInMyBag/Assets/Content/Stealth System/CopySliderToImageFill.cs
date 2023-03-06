using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Stealth_System
{
	[ExecuteAlways]
	public class CopySliderToImageFill : MonoBehaviour
	{
		public Image  image;
		public Slider sliderToCopy;

		
		private void LateUpdate()
		{
			if (image && sliderToCopy)
				image.fillAmount = sliderToCopy.value;
		}
	}
}