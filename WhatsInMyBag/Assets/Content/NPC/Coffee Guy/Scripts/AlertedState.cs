using System.Runtime.InteropServices;
using Content.NPC.Scripts;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public class AlertedState : ICoffeeGuyState
	{
		private CoffeeGuyBehaviour _behaviour;
		private AlertionState _internalState;
		private float _alertReachedTime;
		private readonly AlertionInfo _alertionInfo;

		private ICoffeeGuyState _previousState;
		private ICoffeeGuyState _next;

		private float _lastMovementTime;

		private enum AlertionState
		{
			GoingToAlert,
			AtAlert
		}

		public AlertedState(AlertionInfo alertionInfo, ICoffeeGuyState previousState)
		{
			_alertionInfo = alertionInfo;
			_previousState = previousState;
		}

		public void Init(CoffeeGuyBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
			switch (_internalState)
			{
				case AlertionState.GoingToAlert:
					if (_behaviour.Navigation.RemainingDistance <= 0.2f)
					{
						// reached alert position
						_internalState = AlertionState.AtAlert;
						_alertReachedTime = Time.time;
					}
					
					// hack to allow guys to arrive at an alertion even if something in the path is blocking them
					if(_behaviour.Navigation.IsStopped)
					{
						if (_behaviour.Navigation.RemainingDistance <= _alertionInfo.prop.maxAlertionDistance &&
						    Time.time - _lastMovementTime >= 2.0f)
						{
							// reached alert position
							_internalState = AlertionState.AtAlert;
							_alertReachedTime = Time.time;
						}
					}
					else
					{
						_lastMovementTime = Time.time;
					}

					break;
				case AlertionState.AtAlert:
					// set progress bar
					float normalizedProgress = (Time.time - _alertReachedTime) / _alertionInfo.alertionDuration;
					_behaviour.progressBar.value = normalizedProgress;
					RotateTowardsAlertionObject();
					
					if (Time.time - _alertReachedTime > _alertionInfo.alertionDuration && _next == null)
					{
						_next = new AlertableState(_previousState);
					}

					break;
			}
		}

		public ICoffeeGuyState Next()
		{
			return _next;
		}
		
		private void RotateTowardsAlertionObject () {
			Vector3 direction = (_alertionInfo.alertionObject.transform.position - _behaviour.transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			_behaviour.transform.rotation = Quaternion.Slerp(_behaviour.transform.rotation, lookRotation, Time.deltaTime * 10);
		}

		public void OnEnter()
		{
			float angle = _alertionInfo.prop.alertionAngle / 2;
			float3 offset = math.mul(
				quaternion.Euler(0, math.radians(Random.Range(-angle, + angle) + _alertionInfo.prop.alertionDirection), 0),
				_alertionInfo.alertionObject.transform.forward * Random.Range(_alertionInfo.prop.minAlertionDistance, _alertionInfo.prop.maxAlertionDistance));
			
			_behaviour.Navigation.NavigateTo(_alertionInfo.alertionObject.transform.position + (Vector3)offset);
			_internalState = AlertionState.GoingToAlert;

			_behaviour.face.material = _behaviour.shocked;
			_lastMovementTime = Time.time;
		}

		public void OnExit()
		{
			_behaviour.progressBar.value = 0;
		}
	}
}