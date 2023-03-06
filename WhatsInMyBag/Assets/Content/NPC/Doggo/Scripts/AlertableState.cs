using System;
using UnityEngine;

namespace Content.NPC.Doggo.Scripts
{
	public class AlertableState : IDoggoState
	{
		private DoggoBehaviour _behaviour;
		private IDoggoState _unalarmedState = new PatrolState();
		private BarkingState _barking = new BarkingState();

		private AlertState _currState = AlertState.Unalarmed;
		private float _lastSeenBodyTime;
		
		private enum AlertState
		{
			Unalarmed,
			Alarmed,
		}
		
		public void Init(DoggoBehaviour guy)
		{
			_behaviour = guy;
			_unalarmedState.Init(guy);
			_barking.Init(guy);
		}

		public void Update()
		{
			// if too much time passed since seeing the last dead body, go to unalarmed state
			if (Time.time - _lastSeenBodyTime > _behaviour.timeDelayToUnalarmed)
			{
				if (_currState == AlertState.Alarmed)
				{
					_currState = AlertState.Unalarmed;
					_barking.OnExit();
					_unalarmedState.OnEnter();
				}
			}
			
			
			switch (_currState)
			{
				case AlertState.Unalarmed:
					if (_unalarmedState.Next() != null)
					{
						_unalarmedState.OnExit();
						_unalarmedState = _unalarmedState.Next();
						_unalarmedState.Init(_behaviour);
						_unalarmedState.OnEnter();
					}

					_unalarmedState.Update();
					break;
				case AlertState.Alarmed:
					_barking.Update();
					break;
			}
		}

		public IDoggoState Next()
		{
			return null;
		}

		public string Name()
		{
			switch (_currState)
			{
				case AlertState.Unalarmed:
					return _unalarmedState.Name();
				case AlertState.Alarmed:
					return _barking.Name();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void OnEnter()
		{
			_behaviour.Vision.seenDeadBodyEvent += OnSeenBody;

			_currState = AlertState.Unalarmed;
			_unalarmedState.OnEnter();
		}

		public void OnExit()
		{
			switch (_currState)
			{
				case AlertState.Unalarmed:
					_unalarmedState.OnExit();
					break;
				case AlertState.Alarmed:
					_barking.OnExit();
					break;
			}
			
			_behaviour.Vision.seenDeadBodyEvent -= OnSeenBody;
		}
		
		public void OnSeenBody(Transform body)
		{
			_lastSeenBodyTime = Time.time;

			// go to alarmed state
			if (_currState == AlertState.Unalarmed)
			{
				_currState = AlertState.Alarmed;
				_unalarmedState.OnExit();
				_barking.OnEnter();
			}
		}
	}
}