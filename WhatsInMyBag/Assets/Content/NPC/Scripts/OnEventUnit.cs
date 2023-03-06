using Bolt;
using GameEvents;
using Ludiq;
using UnityEngine;

namespace Content.NPC.Scripts
{
	[UnitCategory("Custom Events")]
	public class OnEventUnit<T> : Unit where T : GameEvent
	{
		[DoNotSerialize]
		public ControlInput input { get; private set; }

		[DoNotSerialize]
		public ControlOutput eventOutput { get; private set; }
		[DoNotSerialize]
		public ControlOutput elseOutput { get; private set; }

		private bool _hasEvent       = false;
		private int  _activatedFrame = 0;
		


		public override void AfterAdd()
		{
			GameEventManager.AddListener<T>(OnRaiseEvent);
			base.AfterAdd();
		}

		public override void BeforeRemove()
		{
			GameEventManager.RemoveListener<T>(OnRaiseEvent);
			base.BeforeRemove();
		}

		protected override void Definition()
		{
			input = ControlInput("In", Enter);
			eventOutput = ControlOutput("On Event");
			elseOutput = ControlOutput("Else");
		}

		public void OnRaiseEvent(T e)
		{
			_hasEvent = true;
			_activatedFrame = Time.frameCount;
		}

		public ControlOutput Enter(Flow flow)
		{
			if (_hasEvent && Mathf.Abs(_activatedFrame - Time.frameCount) <= 1) // we react on events even one frame too late
			{
				_hasEvent = false;
				return eventOutput;
			}
			else
			{
				_hasEvent = false;
				return elseOutput;
			}
		}
	}
}