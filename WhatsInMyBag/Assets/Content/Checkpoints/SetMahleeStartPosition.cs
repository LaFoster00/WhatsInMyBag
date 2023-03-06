using Content.Task_System;
using UnityEngine;

namespace Content.Checkpoints
{
	public class SetMahleeStartPosition : MonoBehaviour
	{
		public Transform mahlee;
		
		public Transform beforeIntroStart;
		public Transform afterIntroStart;

		public Task taskAfterIntro;

		private void Start()
		{
			if (TaskManager.Instance.current == taskAfterIntro || TaskManager.Instance.current == null)
			{
				mahlee.position = afterIntroStart.position;
			}
			else
			{
				mahlee.position = beforeIntroStart.position;
			}
		}
	}
}