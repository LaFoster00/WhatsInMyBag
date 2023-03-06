using System;
using DeadBody;
using UnityEngine;

namespace Content.NPC.Scripts
{
	public class DoggoVision : MonoBehaviour
	{
		[Range(0, 180)] public float sightAngle;

		public float maxSightDistance;

		[Tooltip("Distance where the NPC 'sees' things even behind them")]
		public float allAroundDistance = 1.5f;

		[Tooltip("Gives the position of the eye, important if the NPC should be able to look over half height covers")]
		public Transform eye;

		public Transform player;
		public Vector3 playerLookOffset = new Vector3(0, 1, 0);

		public bool drawDebug;


		public OnSeenThing seenPlayerEvent;
		public OnSeenThing seenDeadBodyEvent;


		public delegate void OnSeenThing(Transform thing);

		private void Start()
		{
			Debug.Assert(eye);
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

			if (DeadBodyInteractable.Instance && LookTowards(DeadBodyInteractable.Instance.transform) &&
			    seenDeadBodyEvent != null)
			{
				seenDeadBodyEvent.Invoke(DeadBodyInteractable.Instance.transform);
			}
			else if (player && LookTowardsPlayer() && seenPlayerEvent != null)
			{
				seenPlayerEvent.Invoke(player);
			}
		}

		private bool LookTowards(Transform target)
		{
			Vector3 lookDirection = target.position - eye.position;

			if (Physics.Raycast(new Ray(eye.position, lookDirection), out RaycastHit hit, maxSightDistance))
			{
				if (hit.transform != target)
				{
					return false;
				}

				if (hit.distance <= allAroundDistance)
				{
					return true;
				}


				// check vision angle
				float angleFactor =
					(Vector3.Dot(eye.forward, lookDirection) + 1.0f) / 2.0f; // goes from 0, behind, to 1, in front
				float minAngleFactor =
					(Vector3.Dot(eye.forward, Quaternion.Euler(0, sightAngle / 2.0f, 0) * eye.forward) + 1.0f) / 2.0f;

				return angleFactor >= minAngleFactor;
			}
			else
			{
				return false;
			}
		}

		private bool LookTowardsPlayer()
		{
			var target = player;
			Vector3 lookDirection = (target.position + playerLookOffset) - eye.position;

			if (Physics.Raycast(new Ray(eye.position, lookDirection), out RaycastHit hit, maxSightDistance))
			{
				if (hit.transform != target)
				{
					if (drawDebug)
						Debug.DrawLine(eye.position, hit.point, Color.white);

					return false;
				}

				if (hit.distance <= allAroundDistance)
				{
					if (drawDebug)
						Debug.DrawLine(eye.position, hit.point, Color.red);

					return true;
				}


				// check vision angle
				float angleFactor =
					(Vector3.Dot(eye.forward, lookDirection) + 1.0f) / 2.0f; // goes from 0, behind, to 1, in front
				float minAngleFactor =
					(Vector3.Dot(eye.forward, Quaternion.Euler(0, sightAngle / 2.0f, 0) * eye.forward) + 1.0f) / 2.0f;

				return angleFactor >= minAngleFactor;
			}
			else
			{
				if (drawDebug)
					Debug.DrawLine(eye.position, hit.point, Color.white);

				return false;
			}
		}
	}
}