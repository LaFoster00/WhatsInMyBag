using UnityEngine;
using UnityEngine.Playables;

namespace Content.Common.Scripts
{
	public class TimeLineTrigger : MonoBehaviour
	{
		public PlayableDirector timeline;
		public string           requiredTag;

		private bool _wasActivated = false;

		private void OnTriggerEnter(Collider other)
		{
			if (!_wasActivated && (requiredTag.Length == 0 || other.CompareTag(requiredTag)))
			{
				timeline.Play();
				_wasActivated = true;
			}
		}
	}
}