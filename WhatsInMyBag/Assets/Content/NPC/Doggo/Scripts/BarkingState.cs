using UnityEngine;

namespace Content.NPC.Doggo.Scripts
{
	public class BarkingState : IDoggoState
	{
		private DoggoBehaviour _behaviour;
		private static readonly int IsBarking = Animator.StringToHash("IsBarking");

		public void Init(DoggoBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
		}

		public IDoggoState Next()
		{
			return null;
		}

		public string Name()
		{
			return "BarkingState";
		}

		public void OnEnter()
		{
			_behaviour.animator.SetBool(IsBarking,true);
			_behaviour.NavAgent.speed = 0;
		}

		public void OnExit()
		{
			_behaviour.animator.SetBool(IsBarking, false);
		}
	}
}