using System;
using System.Linq;
using UnityEngine;

namespace Content.Task_System
{
	public class ControlActivityWhileTask : MonoBehaviour
	{
		public bool stateToSet = true;
		public GameObject[] gobjects = new GameObject[1];

		public Task[] whileTasks = new Task[1];

		private void Update()
		{
			if (gobjects.Length == 0)
				return;


			if (whileTasks.Any(task => task == TaskManager.Instance.current))
			{
				foreach (var gobject in gobjects)
				{
					gobject.SetActive(stateToSet);
				}
			}
			else
			{
				foreach (var gobject in gobjects)
				{
					gobject.SetActive(!stateToSet);
				}
			}
		}
	}
}