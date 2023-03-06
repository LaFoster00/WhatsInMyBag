using UnityEngine;

namespace Content.Task_System
{
	[CreateAssetMenu(fileName = "New Task", menuName = "Task/new Task")]
	public class Task : ScriptableObject
	{
		public string text;
		public Task nextTask;
	}
}