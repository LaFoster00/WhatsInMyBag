using System;
using System.Collections.Generic;
using System.Linq;
using Content.Onboarding.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Content.NPC.Scripts
{
	public class VerkAI : MonoBehaviour
	{
		public Transform restingPlace;

		[Tooltip("Maximum distance to a blood puddle at which Verka starts cleaning it")]
		public float maxCleaningRange = .1f;

		[Tooltip("Base time it takes Verka to clean a puddle")]
		public float cleaningDuration = 10f;

		[Tooltip("Multiplier of the time it takes Verka to clean bigger puddles")]
		public float puddleIntensityFactor = 1f;

		public GameObject cleaningBottle;

		public BloodTrail bloodTrailGenerator;


		[Tooltip("Optional animator")] public Animator animator;

		public Slider progressSlider;

		[Tooltip("Optional activation after intro")]
		public string introName;


		private NavMeshAgent _navMeshAgent;

		private VerkaState _state = VerkaState.GoingToRest;
		private List<GameObject> _puddleObjects;
		private List<float> _puddleSizes;

		enum VerkaState
		{
			GoingToRest,
			Resting,
			FollowingTrail,
			Cleaning
		}

		// cleaning
		private float _cleanStartTime;
		private float _totalCleanDuration;


		private void Start()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();

			Debug.Assert(restingPlace, "Assign a resting place");

			GoToRest();

			bloodTrailGenerator.CorpseBleedingEvent += OnNewTrail;
		}

		List<GameObject> OnNewTrail(List<GameObject> bloodTrail, List<int> bloodIntensity)
		{
			_puddleObjects = bloodTrail;
			_puddleSizes = bloodIntensity.Select(i => i / 10.0f).ToList();

			return null;
		}

		public void Update()
		{
			// don't do anything if her intro has not played yet
			if (introName.Length != 0 && !SaveDoneIntros.HasPlacedIntro(introName))
			{
				return;
			}


			if (_puddleObjects == null || (_state != VerkaState.GoingToRest || _state != VerkaState.Resting) &&
				_puddleObjects.Count == 0)
				StartResting();

			switch (_state)
			{
				case VerkaState.GoingToRest:
					if (_navMeshAgent.remainingDistance < 0.1f)
					{
						StartResting();
					}

					break;
				case VerkaState.Resting:
					if (_puddleObjects != null && _puddleObjects.Count != 0)
					{
						_state = VerkaState.FollowingTrail;
						_navMeshAgent.SetDestination(_puddleObjects.First().transform.position);
					}

					break;
				case VerkaState.FollowingTrail:
					if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < maxCleaningRange)
					{
						StartCleaning(_puddleObjects.First().transform.position, _puddleSizes.First());
					}

					break;
				case VerkaState.Cleaning:
					if (_cleanStartTime + _totalCleanDuration < Time.time)
					{
						OnFinishedCleaning();

						if (progressSlider)
							progressSlider.value = 0;
					}
					else
					{
						progressSlider.value = (Time.time - _cleanStartTime) / _totalCleanDuration;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			if (animator)
			{
				animator.SetBool("IsCleaning", _state == VerkaState.Cleaning);
				animator.SetBool("IsResting", _state == VerkaState.Resting);
			}

			if (cleaningBottle)
			{
				cleaningBottle.SetActive(_state == VerkaState.Cleaning);
			}
		}

		private void StartResting()
		{
			_state = VerkaState.Resting;
		}


		public void StartCleaning(Vector3 position, float size)
		{
			// rotate towards puddle
			transform.rotation = Quaternion.LookRotation(position - transform.position);
			_cleanStartTime = Time.time;
			_totalCleanDuration = cleaningDuration + size * puddleIntensityFactor;

			_state = VerkaState.Cleaning;
		}


		public void OnFinishedCleaning()
		{
			Destroy(_puddleObjects.First());

			_puddleObjects.RemoveAt(0); // done with this puddle, remove it
			_puddleSizes.RemoveAt(0);

			if (_puddleObjects.Count == 0)
			{
				GoToRest();
			}
			else
			{
				_state = VerkaState.FollowingTrail;

				_navMeshAgent.SetDestination(_puddleObjects.First().transform.position);
			}
		}

		private void GoToRest()
		{
			_state = VerkaState.GoingToRest;

			_navMeshAgent.SetDestination(restingPlace.position);
		}
	}
}