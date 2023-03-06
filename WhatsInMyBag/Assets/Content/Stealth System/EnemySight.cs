using System;
using DeadBody;
using GameEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;

namespace Content.Stealth_System
{
	public class EnemySight : MonoBehaviour
	{
		[Header("Settings")] public float alertnessFactor = 1.0f;

		public float maxSightDistance;

		[Tooltip("Distance where the NPC 'sees' things even behind them")]
		public float allAroundDistance = 1.5f;

		[Range(0, 180)] public float sightAngle;

		public AnimationCurve distanceToAlertness;
		public AnimationCurve angleToAlertness;
		
		[Tooltip("How many frames pass between each update")]
		public int runEveryNthFrame = 1;

		public bool changeSpeedWhenAlerted;
		public float speedWhenAlerted = 0.1f;

		[Tooltip("Gives the position of the eye, important if the NPC should be able to look over half height covers")]
		public Transform eye;

		public Transform player;

		public Animator animator;

		public bool drawDebug;


		private bool _seesSuspiciousActivity = false;
		private bool _sawSuspiciousActivity = false;
		private NavMeshAgent _agent;
		private float _previousSpeed;
		
		private int _randomSeed;

		private static Random _rng = Random.CreateFromIndex(642);

		private void Awake()
		{
			_randomSeed = _rng.NextInt() % 8192; // module to avoid overflows
		}


		private void Start()
		{
			Debug.Assert(eye);
			Debug.Assert(runEveryNthFrame >= 1);

			_agent = GetComponent<NavMeshAgent>();
			Debug.Assert((changeSpeedWhenAlerted && _agent) || !changeSpeedWhenAlerted,
				"Stop when alerted was turned on, but GameObject is missing a NavMeshAgent to stop");
		}

		private void Update()
		{
			if (drawDebug)
			{
				Debug.DrawRay(eye.position, eye.forward * maxSightDistance, Color.magenta);
				Debug.DrawRay(eye.position,
					Quaternion.Euler(0, sightAngle / 2.0f, 0) * eye.forward * maxSightDistance,
					Color.green);
				Debug.DrawRay(eye.position,
					Quaternion.Euler(0, -sightAngle / 2.0f, 0) * eye.forward * maxSightDistance,
					Color.green);
			}
			
			if((Time.frameCount + _randomSeed) % runEveryNthFrame != 0)
				return;


			_sawSuspiciousActivity = _seesSuspiciousActivity;
			_seesSuspiciousActivity = false;


			CheckVisionTowards(DeadBodyInteractable.Instance.transform.position);


			if (animator)
			{
				animator.SetBool("SeenSuspicousAction", _seesSuspiciousActivity);
			}

			if (changeSpeedWhenAlerted)
			{
				if (_seesSuspiciousActivity)
				{
					// stop
					_agent.speed = speedWhenAlerted;
				}
				else if (!_seesSuspiciousActivity && _sawSuspiciousActivity)
				{
					// return to normal speed
					_agent.speed = _previousSpeed;
				}
				else
				{
					_previousSpeed = _agent.speed;
				}
			}
		}

		private void OnSeenSuspicousAcitivty()
		{
			AkSoundEngine.PostEvent("Play_Alert", gameObject);
			_seesSuspiciousActivity = true;

			if (!_sawSuspiciousActivity)
			{
				GameEventManager.Raise(new EnemySeesBodyEvent(gameObject));
			}
		}

		private bool CheckVisionTowards(Vector3 target)
		{
			if (math.distance(eye.position, target) > maxSightDistance) return true;
			
			RaycastHit[] hits = Physics.RaycastAll(
				new Ray(eye.position, target - eye.position),
				math.min(maxSightDistance, (target - eye.position).magnitude), int.MaxValue, QueryTriggerInteraction.Ignore);
			Array.Sort(hits, (hit1, hit2) => (int) (1000.0f * (hit1.distance - hit2.distance)));

			if (drawDebug)
			{
				Debug.DrawLine(eye.position, target, Color.red);
			}

			return ProcessVisionRay(hits, target);
		}

		private bool ProcessVisionRay(RaycastHit[] visionHits, Vector3 targetPosition)
		{
			bool hitImportantPart = false;
			float visibilityFactor = 1;


			foreach (RaycastHit currHit in visionHits)
			{
				if ((currHit.collider.CompareTag("Player") && DeadBodyInteractable.Instance.IsInteractable)
				    || currHit.collider.gameObject.layer == LayerMask.NameToLayer("Dead Body")
				    || currHit.collider.gameObject.layer == LayerMask.NameToLayer("No Player Collision"))
				{
					hitImportantPart = true;
					break;
					// we dont iterate further, everything behind the player is not of interest
				}
				else
				{
					// calculate visiblityFactor
					float factor = 1;

					switch (currHit.collider.gameObject.layer)
					{
						case 11: // player
						case 14: // dead body
							goto DoneIterateHits;
						case 15: // props
							factor = 0.5f;
							break;
						case 16: // see through
							factor = 1;
							break;
						default:
							return false;
					}


					visibilityFactor *= factor;
				}
			}

			DoneIterateHits:

			// we see the player
			if (hitImportantPart)
			{
				float alertness = CalculateAlertness(targetPosition);

				if (alertness != 0)
				{
					PlayerAlertMeter.Instance.AddAlertness(visibilityFactor * alertness * Time.deltaTime);
					OnSeenSuspicousAcitivty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public float CalculateAlertness(Vector3 playerPosition)
		{
			float distanceToPlayer = Vector3.Distance(eye.position, playerPosition);

			if (distanceToPlayer < allAroundDistance)
			{
				return alertnessFactor * runEveryNthFrame;
			}

			if (distanceToPlayer > maxSightDistance)
			{
				return 0;
			}


			Vector3 eyeToPlayer = playerPosition - eye.position;
			eyeToPlayer.y = 0;
			eyeToPlayer.Normalize();

			// check angle
			float angleFactor =
				(Vector3.Dot(eye.forward, eyeToPlayer) + 1.0f) / 2.0f; // goes from 0, behind, to 1, in front
			float minAngleFactor =
				(Vector3.Dot(eye.forward, Quaternion.Euler(0, sightAngle / 2.0f, 0) * eye.forward) +
				 1.0f) / 2.0f;


			// map the angleFactor from Range(minAngleFactor, 1) to Range(0, 1)
			float range = 1 - minAngleFactor;
			float normalizedAngleFactor = angleFactor - minAngleFactor;
			normalizedAngleFactor *= 1 / range;
			if (angleFactor < minAngleFactor && distanceToPlayer > allAroundDistance)
			{
				return 0;
			}


			float angleAlertness = angleToAlertness.Evaluate(1 - normalizedAngleFactor);
			float distanceAlertness = distanceToAlertness.Evaluate(distanceToPlayer / maxSightDistance);

			return angleAlertness * distanceAlertness * alertnessFactor * runEveryNthFrame;
		}
	}
}