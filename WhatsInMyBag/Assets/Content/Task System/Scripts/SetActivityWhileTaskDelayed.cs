using System;
using System.Collections;
using System.Linq;
using MiscUtil.Extensions.TimeRelated;
using UnityEngine;

namespace Content.Task_System
{
	public class SetActivityWhileTaskDelayed : MonoBehaviour
	{
		public bool stateToSet = true;
		public GameObject[] gobjects = new GameObject[1];

		public float delay = 0f;

		public Task[] whileTasks = new Task[1];

		private bool _isCoroutineActive = false;

		private void Start()
		{
			UpdateState();
		}

		private void Update()
		{
			UpdateState();
		}

		private void UpdateState()
		{
			if (gobjects.Length == 0 || _isCoroutineActive)
				return;


			bool currentState = whileTasks.Any(task => task == TaskManager.Instance.current);

			// dont need to change
			if (currentState == gobjects[0].activeSelf)
				return;

			if (currentState)
			{
				if (delay != 0)
				{
					StartCoroutine(DelayStateSet(stateToSet));
				}
				else
				{
					foreach (var gobject in gobjects)
					{
						gobject.SetActive(stateToSet);
					}
				}
			}
			else
			{
				if (delay != 0)
				{
					StartCoroutine(DelayStateSet(!stateToSet));
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

		private IEnumerator DelayStateSet(bool state)
		{
			_isCoroutineActive = true;
			yield return new WaitForSeconds(delay);

			foreach (var gobject in gobjects)
			{
				gobject.SetActive(state);
			}

			_isCoroutineActive = false;
		}
	}
}