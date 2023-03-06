using UnityEngine;

namespace Content.Task_System
{
	public class TaskSetter : MonoBehaviour
	{
		public void DestructTaskManager()
		{
			if (TaskManager.Instance != null)
			{
				Destroy(TaskManager.Instance.gameObject);
			}
		}
	}
}