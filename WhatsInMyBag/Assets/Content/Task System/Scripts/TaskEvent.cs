using GameEvents;

namespace Content.Task_System
{
	/// <summary>
	/// Raised when a task is being tried to finish
	/// </summary>
	public class TaskEvent : GameEvent
	{
		public Task BelongingTask;
	}

	/// <summary>
	/// Raised when a new task becomes active
	/// </summary>
	public class NewTaskEvent : GameEvent
	{
		public Task Task;
	}

	/// <summary>
	/// Raised when a task is actually finished
	/// </summary>
	public class TaskFinishedEvent : GameEvent
	{
		public Task FinishedTask;
	}
}