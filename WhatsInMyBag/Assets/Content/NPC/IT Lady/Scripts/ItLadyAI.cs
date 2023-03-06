using System;
using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Interactables.Props;
using Props.Description.Data;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Content.NPC.IT_Lady.Scripts
{
	public class ItLadyAI : MonoBehaviour
	{
		public float repairDuration = 20.0f;

		private NavMeshAgent _navMeshAgent;
		private NavMeshTriangulation _possibleLocations;
		private ITLadyState _state = ITLadyState.Wandering;

		private float _repairStartTime;

		private List<Prop> _machinesToRepair;


		public delegate void MachineBrokeFunc(ITMachine machine);

		public static MachineBrokeFunc OnBrokeMachineDel;

		private enum ITLadyState
		{
			Repairing,
			GoingToBrokenMachine,
			Waiting,
			Wandering
		}

		private void Start()
		{
			_machinesToRepair = new List<Prop>();

			_navMeshAgent = GetComponent<NavMeshAgent>();
			_possibleLocations = NavMesh.CalculateTriangulation();
		}

		private void OnEnable()
		{
			GameEventManager.AddListener<ITMachineBrokeEvent>(OnMachineBroke);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<ITMachineBrokeEvent>(OnMachineBroke);
		}

		private void Update()
		{
			switch (_state)
			{
				case ITLadyState.Repairing:
					if (Time.time > _repairStartTime + repairDuration)
					{
						OnRepairFinished();
					}

					break;
				case ITLadyState.GoingToBrokenMachine:
					if (!_navMeshAgent.pathPending &&
						(_navMeshAgent.isPathStale || _navMeshAgent.remainingDistance < 0.15f))
					{
						StartRepairing();
					}

					break;
				case ITLadyState.Waiting:
					break;
				case ITLadyState.Wandering:
					break;
					DoWander();

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnMachineBroke(ITMachineBrokeEvent e)
		{
			_machinesToRepair.Add(e.Machine);

			if (_state == ITLadyState.GoingToBrokenMachine || _state == ITLadyState.Repairing)
				return;

			_state = ITLadyState.GoingToBrokenMachine;
			_navMeshAgent.SetDestination(_machinesToRepair.First().transform.position);
		}

		private void StartWaiting()
		{
			_state = ITLadyState.Waiting;
		}

		private void StartRepairing()
		{
			_state = ITLadyState.Repairing;
			_repairStartTime = Time.time;
		}

		private void DoWander()
		{
			if (_navMeshAgent.remainingDistance < 1.0f)
			{
				WanderToNextPoint();
			}
		}

		private void WanderToNextPoint()
		{
			Vector3 randomPosition = _possibleLocations.vertices[Random.Range(0, _possibleLocations.vertices.Length)];

			while (Vector3.Distance(transform.position, randomPosition) < 5.0f)
			{
				randomPosition = _possibleLocations.vertices[Random.Range(0, _possibleLocations.vertices.Length)];
			}

			_navMeshAgent.SetDestination(randomPosition);
		}

		private void OnRepairFinished()
		{
			#region Repair Machine
			
			Prop machine = _machinesToRepair[0];
			if (machine.GetCustomDataGeneric(out CD_Destructable data))
			{
				if (data.ExplosionVFX)
				{
					data.ExplosionVFX.StopEffect();
				}

				data.Damaged = false;
			}
			machine.IsInteractable = true;
			
			#endregion
			
			_machinesToRepair.RemoveAt(0);

			if (_machinesToRepair.Count == 0)
			{
				StartWaiting();
			}
			else
			{
				_state = ITLadyState.GoingToBrokenMachine;
				_navMeshAgent.SetDestination(_machinesToRepair.First().transform.position);
			}
		}
	}
}