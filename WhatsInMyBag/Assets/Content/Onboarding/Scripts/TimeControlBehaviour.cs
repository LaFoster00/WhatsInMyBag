using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Content.Onboarding.Scripts
{
	[Serializable]
	public class TimeControlBehaviour : PlayableBehaviour
	{
		[SerializeField]
		private float timeScale;

		[SerializeField]
		private bool resetTimeAfterEnd = true;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			Time.timeScale = timeScale * info.weight + Time.timeScale * (1 - info.weight);
		}

		public override void OnGraphStop(Playable playable)
		{
			if (resetTimeAfterEnd)
				Time.timeScale = 1;
		}
	}
}