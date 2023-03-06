using UnityEngine;

namespace Content.NPC.Doggo.Scripts
{
	public class PatrolState : IDoggoState
	{
		private DoggoBehaviour _behaviour;

		private int        _currPatrolPoint      = 0;
		private bool       _isPatrollingForwards = false;

		private IDoggoState _next;
		
		public void Init(DoggoBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
			_behaviour.NavAgent.speed =_behaviour. walkSpeed;

			if (_behaviour.NavAgent.remainingDistance <= 0.2f)
			{
				GotoNextPatrolPoint();
			}
		}

		public IDoggoState Next()
		{
			return _next;
		}

		public string Name()
		{
			return "PatrolState";
		}

		public void OnEnter()
		{
			_currPatrolPoint = 0;
			_behaviour.Vision.seenPlayerEvent += OnSeenMahlee;
			
			_behaviour.NavAgent.speed = _behaviour.walkSpeed;
		}

		public void OnExit()
		{
			_behaviour.Vision.seenPlayerEvent -= OnSeenMahlee;
		}

		public void OnSeenMahlee(Transform player)
		{
			_next = new FollowState();
		}
		
		private void GotoNextPatrolPoint()
		{
			if (_isPatrollingForwards)
				_currPatrolPoint++;
			else
				_currPatrolPoint--;

			if (_currPatrolPoint >= _behaviour.route.Points.Length)
			{
				_isPatrollingForwards = !_isPatrollingForwards;
				_currPatrolPoint = _behaviour.route.Points.Length - 2;
			}
			else if (_currPatrolPoint <= 0)
			{
				_isPatrollingForwards = !_isPatrollingForwards;
				_currPatrolPoint = 1;
			}

			_behaviour.NavAgent.SetDestination(_behaviour.route.Points[_currPatrolPoint].position);
		}
	}
}