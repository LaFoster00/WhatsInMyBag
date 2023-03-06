using DeadBody;
using GameEvents;
using UnityEngine;

namespace Content.Checkpoints
{
	public class Checkpoint : MonoBehaviour
	{
		public int checkpointID;

		public int CheckPointID => checkpointID;

		private bool _hasBeenActivated;
		
		private void OnTriggerEnter(Collider other)
		{
			if (!_hasBeenActivated && other.CompareTag("Player") && !DeadBodyInteractable.Instance.IsInteractable)
			{
				ActivateCheckpoint();
			}
		}

		public void ActivateCheckpoint()
		{
			CheckpointsManager.Instance.SetActiveCheckpoint(this);
			AkSoundEngine.PostEvent("Play_Checkpoint", gameObject);
			_hasBeenActivated = true;
			GameEventManager.Raise(new CheckpointActivatedEvent(gameObject));
		}
	}
}