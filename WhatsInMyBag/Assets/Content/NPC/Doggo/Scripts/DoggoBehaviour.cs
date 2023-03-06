using System;
using Cinemachine;
using Content.NPC.Scripts;
using MiscUtil.Extensions.TimeRelated;
using UnityEngine;
using UnityEngine.AI;

namespace Content.NPC.Doggo.Scripts
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(DoggoVision))]
	public class DoggoBehaviour : MonoBehaviour
	{
		public float walkSpeed = 0.5f;
		public float runSpeed = 1.5f;

		[Tooltip("How many seconds pass until the doggo stops barking since last seeing a dead body")]
		public float timeDelayToUnalarmed = 0.5f;

		[Tooltip("How many seconds pass where mahlee was not seen while following her to get back to its patrol routine")]
		public float delayToUnfollow = 1.5f;

		public float followMahleeDistance = 2.0f;

		public float maxMahleeMovementDistance = 4.0f;

		public Transform mahlee;
		public Transform security;
		public PatrolRoute route;
		public Animator animator;
		public Transform mouth;

		[NonSerialized]
		public float TimeSinceLastFetch;

		/// <summary>
		/// Small hack to protect against too quickly repeating fetch play
		/// </summary>
		public static float MinTimeBetweenLastFetch = 2.0f;

		public DoggoVision Vision { get; private set; }

		public NavMeshAgent NavAgent { get; private set; }

		public IDoggoState CurrState { get; private set; }
		
		public IInteractable ItemToFetch;

		private void Start()
		{
			Debug.Assert(mahlee, "Need to assign Mahlee");
			Debug.Assert(security, "Need to assign security");
			Debug.Assert(mouth, "Need to assign a mouth transform");

			NavAgent = GetComponent<NavMeshAgent>();
			Vision = GetComponent<DoggoVision>();

			CurrState = new AlertableState();
			CurrState.Init(this);
			CurrState.OnEnter();
		}

		private void Update()
		{
			CheckNextState();

			CurrState.Update();
		}

		private void CheckNextState()
		{
			if (CurrState.Next() != null)
			{
				CurrState.OnExit();
				CurrState.Next().Init(this);
				CurrState = CurrState.Next();
				CurrState.OnEnter();
			}
		}
	}
}