using System;
using Content.NPC.Doggo.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Content.NPC.Scripts
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class SecurityGuyBehaviour : MonoBehaviour
	{
		public Vector3 followOffset = new Vector3(1, 0, 0);
		public float runSpeed = 6;
		public float walkSpeed = 2;

		public DoggoBehaviour doggo;
		public Animator animator;

		private NavMeshAgent _navAgent;
		private SecurityState _state;

		private Vector3 _alertionPosition;

		private enum SecurityState
		{
			FollowingDog,
			TriggeredByDog,
			LookingAround,
			WaitingForDog,
			Alerted,
		}

		private void Start()
		{
			Debug.Assert(doggo, "Security is missing his doggo ;_;");
			_navAgent = GetComponent<NavMeshAgent>();

			StartFollowing();
		}

		private void Update()
		{
			// state transitions that are triggered by doggo
			// pretty dirty, but it works as a hack...
			var dogState = doggo.CurrState.Name();
			switch (dogState)
			{
				case "PatrolState":
					StartFollowing();
					break;
				case "FollowState":
					StartWaitingForDog();
					break;
				case "BarkingState":
					StartTriggered();
					break;
			}
			
			bool isRunning = doggo.CurrState.Name() == "BarkingState" || _state == SecurityState.Alerted;
			_navAgent.speed = isRunning ? runSpeed : walkSpeed;


			switch (_state)
			{
				case SecurityState.FollowingDog:
					if (doggo.CurrState.Name() != "PatrolState")
					{
						StartTriggered();
						break;
					}

					if ((Time.frameCount + 4) % 10 == 0) // + 4 to not load all pathfinding onto one frame
					{
						_navAgent.SetDestination(doggo.transform.position + followOffset);
					}

					break;
				case SecurityState.TriggeredByDog:
					if (Vector3.Distance(transform.position, doggo.transform.position) <= 0.3f)
					{
						StartLookingAround();
					}


					if ((Time.frameCount + 4) % 10 == 0)
					{
						_navAgent.SetDestination(doggo.transform.position);
					}

					break;
				case SecurityState.LookingAround:
					break;
				case SecurityState.WaitingForDog:
					if (Vector3.Distance(transform.position, doggo.transform.position) <= followOffset.magnitude + 0.3f)
					{
						StartFollowing();
					}

					break;
				case SecurityState.Alerted:
					if (Time.frameCount % 16 == 0)
					{
						_navAgent.SetDestination(_alertionPosition);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (animator)
			{
				animator.SetBool("IsLookingAround", _state == SecurityState.LookingAround);
			}
		}

		private void StartFollowing()
		{
			if (_state == SecurityState.FollowingDog)
				return;

			_navAgent.SetDestination(doggo.transform.position + followOffset);
			_state = SecurityState.FollowingDog;
		}

		private void StartTriggered()
		{
			if (_state == SecurityState.TriggeredByDog)
				return;

			_state = SecurityState.TriggeredByDog;
			_navAgent.SetDestination(doggo.transform.position);
		}

		private void StartWaitingForDog()
		{
			if (_state == SecurityState.WaitingForDog)
				return;

			_state = SecurityState.WaitingForDog;
		}

		private void StartLookingAround()
		{
			if (_state == SecurityState.LookingAround)
				return;

			_state = SecurityState.LookingAround;
		}

		public void OnAlert(AlertionInfo alertionInfo)
		{
			_alertionPosition = alertionInfo.alertionObject.transform.position;
			_state = SecurityState.Alerted;
		}
	}
}