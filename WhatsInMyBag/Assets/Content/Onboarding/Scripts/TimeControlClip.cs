using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Content.Onboarding.Scripts
{
	[Serializable]
	public class TimeControlClip : PlayableAsset, ITimelineClipAsset
	{
		[SerializeField]
		private TimeControlBehaviour template = new TimeControlBehaviour();
		
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<TimeControlBehaviour>.Create(graph, template);
		}

		public ClipCaps clipCaps => ClipCaps.Blending;
	}
}