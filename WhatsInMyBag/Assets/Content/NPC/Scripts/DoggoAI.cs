using System;
using GameEvents;
using UnityEngine;
using UnityEngine.AI;

namespace Content.NPC.Scripts
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(DoggoVision))]
	public class DoggoAI : MonoBehaviour
	{
		public float walkSpeed = 1;
		public float runSpeed  = 4;

		[Tooltip("The distance doggo keeps when following Mahlee")]
		public float maxDistanceFromMahlee = 1.3f;

		[Tooltip("The route to move along")]
		public PatrolRoute route;

		[Tooltip("Optional animator")]
		public Animator animator;

		[Tooltip("The Security guy that walks beside this doggo")]
		public Transform security;

		[Tooltip("The position where this grabs the items to bring it to ppl")]
		public Transform mouth;


		[Header("Play Fetch")]
		[Tooltip("Delay in seconds where the doggo just stays at his place")]
		public float delayAfterItemBringing = 2;

		[Tooltip("How much Mahlee is allowed to moved while still bringing the thrown item to her")]
		public float maxMahleeMovementForFetch = 2.0f;


		private NavMeshAgent _navAgent;


		private int        _currPatrolPoint      = 0;
		private bool       _isPatrollingForwards = false;
		private bool       _seesBody             = false;
		private DoggoState _previousState        = DoggoState.Patrol;
		private DoggoState _internalState        = DoggoState.Patrol;
		private GameObject _itemToBring;
		private bool       _arrivedAtTarget = false;
		private float      _waitStartTime;
		private Vector3    _fetchMahleePosition;
		private Transform  _bringTarget;

		private Transform _player;

		private const float MinReachedDistance = 0.2f;

		public DoggoState State
		{
			get => _internalState;
			private set
			{
				_previousState = _internalState;
				_internalState = value;
			}
		}

		public enum DoggoState
		{
			Patrol,
			FollowMahlee,
			GetItem,
			BringItem,
			Barking,
		}

		private void Start()
		{
			_navAgent = GetComponent<NavMeshAgent>();

			var vision = GetComponent<DoggoVision>();
			vision.seenPlayerEvent += OnSeenMahlee;
			vision.seenDeadBodyEvent += OnSeenBody;

			StartPatrol();
		}

		private void OnEnable()
		{
			GameEventManager.AddListener<InteractableImpactEvent>(OnItemThrown);
			GameEventManager.AddListener<ItemPickedUpEvent>(OnItemPickedUp);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<InteractableImpactEvent>(OnItemThrown);
			GameEventManager.RemoveListener<ItemPickedUpEvent>(OnItemPickedUp);
		}

		private void Update()
		{
			switch (State)
			{
				case DoggoState.Patrol:
					_navAgent.speed = walkSpeed;

					if (_navAgent.remainingDistance <= MinReachedDistance)
					{
						GotoNextPatrolPoint();
					}

					break;
				case DoggoState.FollowMahlee:
					_navAgent.speed = runSpeed;

					if (Time.frameCount % 10 == 0)
					{
						// hold some distance from mahlee
						GotoMahlee();
					}

					break;
				case DoggoState.GetItem:
					_navAgent.speed = runSpeed;

					if (!_itemToBring)
					{
						// if somehow the item gets lost
						StartFollowMahlee();
						break;
					}

					if (_navAgent.remainingDistance <= MinReachedDistance)
					{
						StartBringItem();
					}
					else if (Time.frameCount % 15 == 0)
					{
						_navAgent.SetDestination(_itemToBring.transform.position);
					}

					break;
				case DoggoState.BringItem:
					_navAgent.speed = (walkSpeed + runSpeed) / 2.0f;

					if (!_itemToBring)
					{
						// if somehow the item gets lost
						StartFollowMahlee();
						break;
					}

					if (_navAgent.remainingDistance <= maxMahleeMovementForFetch)
					{
						if (!_arrivedAtTarget)
						{
							_arrivedAtTarget = true;
							_waitStartTime = Time.time;
						}
						else if (delayAfterItemBringing < Time.time - _waitStartTime)
						{
							if (_bringTarget == _player)
								OnReachedMahleeWithItem();
							else
								OnReachedSecurityWithItem();
						}
					}
					else if (Time.frameCount % 10 == 0)
					{
						// check if player moved away from fetch playing
						if (!_player || (_bringTarget != security &&
						                 maxMahleeMovementForFetch < Vector3.Distance(_fetchMahleePosition, _player.transform.position)))
						{
							_bringTarget = security;
						}

						if (_bringTarget == _player)
						{
							GotoMahlee();
						}
						else
						{
							_navAgent.SetDestination(_bringTarget.position);
						}
					}

					break;
				case DoggoState.Barking:
					_navAgent.speed = 0;

					if (!_seesBody)
					{
						State = _previousState;
					}
					

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			if (animator)
			{
				animator.SetBool("IsBarking", State == DoggoState.Barking);
				animator.SetBool("IsFollowingMahlee", State == DoggoState.FollowMahlee);
			}

			_seesBody = false; // reset this as it gets updated next frame
		}

		void OnItemThrown(InteractableImpactEvent e)
		{
			if (State == DoggoState.FollowMahlee)
			{
				_itemToBring = e.Interactable.GameObject;
				if (_player)
					_fetchMahleePosition = _player.position;
				StartGetItem();
			}
		}

		void OnItemPickedUp(ItemPickedUpEvent e)
		{
			// check if the picked up item is the one the doggo wants to get
			if (State != DoggoState.GetItem || !_itemToBring)
				return;

			if (e.Item.GameObject == _itemToBring)
			{
				StartFollowMahlee();
			}
		}

		public void OnSeenMahlee(Transform player)
		{
			if (State == DoggoState.Patrol)
			{
				_player = player;
				StartFollowMahlee();
			}
		}

		public void OnSeenBody(Transform body)
		{
			_seesBody = true;
			StartBarking();
		}

		private void GotoNextPatrolPoint()
		{
			if (_isPatrollingForwards)
				_currPatrolPoint++;
			else
				_currPatrolPoint--;

			if (_currPatrolPoint >= route.Points.Length)
			{
				_isPatrollingForwards = !_isPatrollingForwards;
				_currPatrolPoint = route.Points.Length - 2;
			}
			else if (_currPatrolPoint <= 0)
			{
				_isPatrollingForwards = !_isPatrollingForwards;
				_currPatrolPoint = 1;
			}

			_navAgent.SetDestination(route.Points[_currPatrolPoint].position);
		}

		private void StartPatrol()
		{
			if (State == DoggoState.Patrol)
				return;

			State = DoggoState.Patrol;
			_navAgent.SetDestination(route.Points[_currPatrolPoint].position);
		}

		private void StartFollowMahlee()
		{
			if (State == DoggoState.FollowMahlee)
				return;

			State = DoggoState.FollowMahlee;
			GotoMahlee();
		}

		private void StartGetItem()
		{
			if (State == DoggoState.GetItem)
				return;

			State = DoggoState.GetItem;

			Debug.Assert(_itemToBring);
			_navAgent.SetDestination(_itemToBring.transform.position);
		}

		private void StartBringItem()
		{
			if (State == DoggoState.BringItem)
				return;

			PickupItem();
			_arrivedAtTarget = false;
			_bringTarget = _player ? _player : security;

			State = DoggoState.BringItem;
			_navAgent.SetDestination(security.transform.position);
		}

		private void StartBarking()
		{
			if (State == DoggoState.Barking)
				return;

			State = DoggoState.Barking;
		}

		private void GotoMahlee()
		{
			var dirToMahlee = _player.position - transform.position;
			dirToMahlee.y = 0;
			dirToMahlee.Normalize();

			_navAgent.SetDestination(_player.position - dirToMahlee * maxDistanceFromMahlee);
		}

		private void OnReachedSecurityWithItem()
		{
			LetItemGo();
			StartPatrol();
		}

		private void OnReachedMahleeWithItem()
		{
			LetItemGo();
			StartFollowMahlee();
		}

		private void PickupItem()
		{
			_itemToBring.transform.position = mouth.position;
			_itemToBring.transform.SetParent(mouth, true);
			var rb = _itemToBring.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.isKinematic = true;
			}
		}

		private void LetItemGo()
		{
			_itemToBring.transform.SetParent(null);
			var rb = _itemToBring.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.isKinematic = false;
			}
		}
	}
}