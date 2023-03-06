using Content.NPC.Scripts;
using UnityEngine;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public class AlertableState : ICoffeeGuyState
	{
		private CoffeeGuyBehaviour _behaviour;

		private ICoffeeGuyState _currState = new RoutineState();
		private AlertedState _next;
		
		public AlertableState()
		{}

		public AlertableState(ICoffeeGuyState nextState)
		{
			_currState = nextState;
		}
		
		public void Init(CoffeeGuyBehaviour guy)
		{
			_behaviour = guy;
			
			_currState.Init(guy);
		}

		public void Update()
		{
			if (_currState.Next() != null)
			{
				_currState.OnExit();

				_currState.Next().Init(_behaviour);
				_currState = _currState.Next();

				_currState.OnEnter();
			}
			
			_currState.Update();
		}

		public ICoffeeGuyState Next()
		{
			return _next;
		}

		public void OnEnter()
		{
			_next = null;
			_currState.OnEnter();
		}

		public void OnExit()
		{
			_currState.OnExit();
		}

		public void Alert(AlertionInfo alertionInfo)
		{
			_next = new AlertedState(alertionInfo, _currState);
		}
	}
}