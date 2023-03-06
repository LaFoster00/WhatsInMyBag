using System;
using GameEvents;
using UnityEngine;
using UnityEngine.Playables;

namespace Content.Checkpoints
{
	public class PlayOnCheckpointSet : MonoBehaviour
	{
		public PlayableDirector timeline;

		private void OnEnable()
		{
			GameEventManager.AddListener<CheckpointActivatedEvent>(OnCheckpoint);
		}
		
		private void OnDisable()
		{
			GameEventManager.RemoveListener<CheckpointActivatedEvent>(OnCheckpoint);
		}

		private void OnCheckpoint(CheckpointActivatedEvent e)
		{
			timeline.Play();
		}
	}
}