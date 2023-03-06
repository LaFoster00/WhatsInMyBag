using UnityEngine;
using UnityEngine.AI;
using USCSL;

namespace Content.NPC.Scripts
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class UpdateAnimatorFromNavAgent : MonoBehaviour
	{
		public Animator animator;
		private SmoothDampFloat _speed;


		private NavMeshAgent _agent;

		private static readonly int Velocity = Animator.StringToHash("Velocity");

		private void Start()
		{
			_agent = GetComponent<NavMeshAgent>();
			_speed = new SmoothDampFloat(0);
		}

		private void Update()
		{
			_speed.Update(_agent.velocity.magnitude, 0.1f);

			if (animator)
				animator.SetFloat(Velocity, _speed);
		}
	}
}