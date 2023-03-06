using UnityEngine;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public class CoffeeState : ICoffeeGuyState
	{
		private CoffeeGuyBehaviour _behaviour;
		private float _startTime;
		private ICoffeeGuyState _next;
		private bool _isMoving;
		
		public void Init(CoffeeGuyBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
			if (_isMoving)
			{
				var navAgent = _behaviour.Navigation;
				if ( navAgent.RemainingDistance <= navAgent.Speed * Time.deltaTime * 2)
				{
					_isMoving = false;
					_startTime = Time.time;
				}
			}
			else
			{
				if(Time.time - _startTime < _behaviour.coffeeMakingDuration)
				{
					float normalizedProgress = (Time.time - _startTime) / _behaviour.coffeeMakingDuration;

					_behaviour.progressBar.value = normalizedProgress;
				}
				else
				{
					_next = new WorkingState();
				}
			}
		}

		public ICoffeeGuyState Next()
		{
			return _next;
		}

		public void OnEnter()
		{
			_isMoving = true;
			_behaviour.Navigation.NavigateTo(_behaviour.coffeeMachine, 1.0f);
		}

		public void OnExit()
		{
			_behaviour.progressBar.value = 0;
		}
	}
}