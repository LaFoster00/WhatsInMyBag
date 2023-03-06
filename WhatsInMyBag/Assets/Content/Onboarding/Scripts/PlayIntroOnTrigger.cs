using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

namespace Content.Onboarding.Scripts
{
	public class PlayIntroOnTrigger : MonoBehaviour
	{
		public PlayableDirector timeline;
		public string name;

		private bool _wasActivated = false;

		private void OnTriggerEnter(Collider other)
		{
			if (!_wasActivated && SaveDoneIntros.Instance.CanPlayIntro(name) && other.CompareTag("Player"))
			{
				timeline.Play();
				_wasActivated = true;

				GameEventManager.Raise(new PlayedIntroEvent() {name = name});
			}
		}
	}
}