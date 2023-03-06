using System;
using UnityEngine;

namespace Content.Task_System
{
	public class ActiveWhileTask : MonoBehaviour
	{
		public bool stateToSet = true;
		public Task onTask;

		public GameObject objectToSet;

		private void Update()
		{
			if (TaskManager.Instance.current == onTask)
			{
				objectToSet.SetActive(stateToSet);	
			}
			else
			{
				objectToSet.SetActive(!stateToSet);
			}
		}
	}
}