using System;
using TMPro;
using UnityEngine;

namespace Content.Task_System
{
	public class TaskUI : MonoBehaviour
	{
		public TMP_Text taskText;
		public Canvas taskCanvas;

		public TaskManager manager;

		private void LateUpdate()
		{
			if (manager.current)
			{
				taskCanvas.gameObject.SetActive(true);

				taskText.text = manager.current.text.Replace("<br>", "\n"); // replace to add line breaks
			}
			else
			{
				if (taskCanvas.gameObject.activeSelf)
				{
					taskCanvas.gameObject.SetActive(false);
					taskText.text = "";
				}
			}
		}
	}
}