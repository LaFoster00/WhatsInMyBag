using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace Content.NPC.Scripts
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class NavigationWithArrival : MonoBehaviour
	{
		public int frameAccuracy = 10;


		private NavMeshAgent _agent;

		private NavigationStatus _status = NavigationStatus.None;
		private Vector3 _target;
		private float _arrivalDistance;
		
		public float RemainingDistance
		{
			get
			{
				if (_agent.pathPending)
				{
					return float.MaxValue;
				}

				return _status switch
				{
					NavigationStatus.ToArrival => _agent.remainingDistance + _arrivalDistance,
					NavigationStatus.ToTarget => _agent.remainingDistance,
					_ => 0
				};
			}
		}

		public bool IsStopped => _agent.speed <= 0.06f;

		public float Speed => _agent.speed;

		private enum NavigationStatus
		{
			ToArrival,
			ToTarget,
			None
		}

		private void Start()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			//TODO Was genau ist der nutzen hier von?
			if (_status == NavigationStatus.ToArrival && !_agent.pathPending &&
			    _agent.remainingDistance <= _agent.speed * Time.deltaTime * frameAccuracy)
			{
				_status = NavigationStatus.ToTarget;
				_agent.SetDestination(_target); 
			}
		}

		public void NavigateTo(Vector3 position, Vector3 arrivalVector)
		{
			var arrivalPoint = position + arrivalVector;
			_agent.SetDestination(arrivalPoint);

			_target = position;
			_arrivalDistance = arrivalVector.magnitude;
			_status = NavigationStatus.ToArrival;
		}

		public void NavigateTo(Vector3 position, Vector3 direction, float arrivalDistance)
		{
			var arrivalPoint = position + direction.normalized * arrivalDistance;
			_agent.SetDestination(arrivalPoint);

			_target = position;
			_arrivalDistance = arrivalDistance;
			_status = NavigationStatus.ToArrival;
		}

		public void NavigateTo(Transform trans, float arrivalDistance)
		{
			var arrivalPoint = trans.position - trans.forward * arrivalDistance;
			_agent.SetDestination(arrivalPoint);

			_target = trans.position;
			_arrivalDistance = arrivalDistance;
			_status = NavigationStatus.ToArrival;
		}

		public void NavigateTo(Vector3 position)
		{
			_agent.SetDestination(position);
			_status = NavigationStatus.ToTarget;
		}
	}
}