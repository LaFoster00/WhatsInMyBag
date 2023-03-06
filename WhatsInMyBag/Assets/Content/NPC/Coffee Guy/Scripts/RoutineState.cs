using UnityEngine;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public class RoutineState : ICoffeeGuyState
	{
		private ToiletState _toiletState = new ToiletState();
		private ICoffeeGuyState _routineState;
		private bool _isOnToilet = false;

		private CoffeeGuyBehaviour _behaviour;
		private float _lastToiletTime = 0;

		public RoutineState()
		{
			// choose a state randomly
			if (Random.value <= 0.5f)
			{
				_routineState = new WorkingState();
			}
			else
			{
				_routineState = new CoffeeState();
			}
			
			_lastToiletTime = Time.time + Random.Range(0, 120);
		}


		public void Init(CoffeeGuyBehaviour guy)
		{
			_behaviour = guy;

			_toiletState.Init(guy);
			_routineState.Init(guy);
		}

		public void Update()
		{
			// state changes
			if (_isOnToilet && _toiletState.IsDone)
			{
				_isOnToilet = false;
				_lastToiletTime = Time.time;

				_routineState.OnEnter();
				_toiletState.OnExit();
			}

			if (_behaviour.toilets && !_isOnToilet &&
			    Time.time - _lastToiletTime > _behaviour.peeingFrequency)
			{
				_isOnToilet = true;

				_routineState.OnExit();
				_toiletState.OnEnter();
			}


			if (_isOnToilet)
			{
				_toiletState.Update();
			}
			else
			{
				CheckNextRoutineState();

				_routineState.Update();
			}
		}

		private void CheckNextRoutineState()
		{
			if (_routineState.Next() != null)
			{
				_routineState.OnExit();

				_routineState.Next().Init(_behaviour);
				_routineState = _routineState.Next();

				_routineState.OnEnter();
			}
		}

		public ICoffeeGuyState Next()
		{
			return null; // no other state possible
		}

		public void OnEnter()
		{
			_lastToiletTime = Time.time;

			if (_isOnToilet)
			{
				_toiletState.OnEnter();
			}
			else
			{
				_routineState.OnEnter();
			}
		}

		public void OnExit()
		{
			if (_isOnToilet)
			{
				_toiletState.OnExit();
			}
			else
			{
				_routineState.OnExit();
			}
		}
	}
}