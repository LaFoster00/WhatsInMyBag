using UnityEngine;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public class ToiletState : ICoffeeGuyState
	{
		private CoffeeGuyBehaviour _behaviour;

		private float _startTime;
		private ICoffeeGuyState _next;
		private bool _isMoving;
		private ICoffeeGuyState _previous;
		
		public bool IsDone { get; private set; }

		public void Init(CoffeeGuyBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
			if (_isMoving)
			{
				var navAgent = _behaviour.Navigation;
				if (navAgent.RemainingDistance <= .3f)
				{
					_isMoving = false;
					_startTime = Time.time;

					_behaviour.face.material = _behaviour.happy;
				}
			}
			else
			{
				if (Time.time - _startTime < _behaviour.peeingDuration)
				{
					float normalizedProgress = (Time.time - _startTime) / _behaviour.peeingDuration;

					_behaviour.progressBar.value = normalizedProgress;
				}
				else
				{
					IsDone = true;
				}
			}
		}

		public ICoffeeGuyState Next()
		{
			return _next;
		}

		public void OnEnter()
		{
			IsDone = false;
			_isMoving = true;
			_behaviour.Navigation.NavigateTo(_behaviour.toilets, 1.0f);
		}

		public void OnExit()
		{
			_behaviour.progressBar.value = 0;
		}
	}
}