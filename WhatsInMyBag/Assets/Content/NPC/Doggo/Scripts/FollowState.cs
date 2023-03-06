using GameEvents;
using UnityEngine;

namespace Content.NPC.Doggo.Scripts
{
	public class FollowState : IDoggoState
	{
		private DoggoBehaviour _behaviour;
		private IDoggoState _next;

		private float _lastSeenMahleeTime;

		public void Init(DoggoBehaviour guy)
		{
			_behaviour = guy;
		}

		public void Update()
		{
			// we lost mahlee
			if (_behaviour.delayToUnfollow < Time.time - _lastSeenMahleeTime)
			{
				_next = new PatrolState();
			}

			if (Time.frameCount % 10 == 0)
			{
				Vector3 mahleePos = _behaviour.mahlee.position;
				mahleePos.y = _behaviour.transform.position.y;


				// hold some distance from mahlee
				if (Vector3.Distance(mahleePos, _behaviour.transform.position) > _behaviour.followMahleeDistance * 1.1f)
					GotoMahlee();
			}
		}

		public IDoggoState Next()
		{
			return _next;
		}

		public string Name()
		{
			return "FollowState";
		}

		public void OnEnter()
		{
			_lastSeenMahleeTime = Time.time;
			_behaviour.Vision.seenPlayerEvent += OnSeenMahlee;

			_behaviour.NavAgent.speed = _behaviour.runSpeed;

			GameEventManager.AddListener<InteractableImpactEvent>(OnItemThrown);
		}

		public void OnExit()
		{
			_behaviour.Vision.seenPlayerEvent -= OnSeenMahlee;

			GameEventManager.RemoveListener<InteractableImpactEvent>(OnItemThrown);
		}

		public void OnSeenMahlee(Transform player)
		{
			_lastSeenMahleeTime = Time.time;
		}

		private void GotoMahlee()
		{
			var dirToMahlee = _behaviour.mahlee.position - _behaviour.transform.position;
			dirToMahlee.y = 0;
			dirToMahlee.Normalize();

			_behaviour.NavAgent.SetDestination(_behaviour.mahlee.position -
				dirToMahlee * _behaviour.followMahleeDistance);
		}

		void OnItemThrown(InteractableImpactEvent e)
		{
			if (Time.time - _behaviour.TimeSinceLastFetch <= DoggoBehaviour.MinTimeBetweenLastFetch)
				return;

			_behaviour.ItemToFetch = e.Interactable;
			_next = new FetchState();
			_next.Init(_behaviour);

			_behaviour.TimeSinceLastFetch = Time.time;
		}
	}
}