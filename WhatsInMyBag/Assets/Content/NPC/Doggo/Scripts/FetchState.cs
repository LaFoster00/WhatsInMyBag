using System;
using System.Collections;
using UnityEngine;

namespace Content.NPC.Doggo.Scripts
{
	public class FetchState : IDoggoState
	{
		private DoggoBehaviour _behaviour;
		private IDoggoState _next;

		private FetchAction _currAction;

		private Vector3 _mahleeThrowPosition;

		private enum FetchAction
		{
			ToItem,
			ToMahlee
		}

		public void Init(DoggoBehaviour guy)
		{
			_behaviour = guy;
		}


		public void Update()
		{
			switch (_currAction)
			{
				case FetchAction.ToItem:
					if (!_behaviour.ItemToFetch.IsInteractable)
					{
						_next = new FollowState();
						break;
					}
						
					
					if (Time.frameCount % 4 + 5 == 0)
					{
						_behaviour.NavAgent.SetDestination(_behaviour.ItemToFetch.GameObject.transform.position);
					}

					// are at the item
					if (!_behaviour.NavAgent.pathPending && _behaviour.NavAgent.remainingDistance <= 0.04f)
					{
						PickupItem();
						
						_currAction = FetchAction.ToMahlee;
						_behaviour.NavAgent.SetDestination(_behaviour.mahlee.position);
					}

					break;
				case FetchAction.ToMahlee:
					if (_behaviour.maxMahleeMovementDistance >
						Vector3.Distance(_behaviour.transform.position, _behaviour.mahlee.position))
					{
						_next = new PatrolState();
					}
					
					// we follow mahlee when we arrived at her
					if (!_behaviour.NavAgent.pathPending &&
					    _behaviour.NavAgent.remainingDistance <= _behaviour.followMahleeDistance)
					{
						_next = new FollowState();
					}
					
					if (Time.frameCount % 15 + 5 == 0)
					{
						_behaviour.NavAgent.SetDestination(_behaviour.mahlee.position);
					}

					break;
			}
		}

		public IDoggoState Next()
		{
			return _next;
		}

		public string Name()
		{
			return "FetchState";
		}

		public void OnEnter()
		{
			_currAction = FetchAction.ToItem;

			_behaviour.NavAgent.speed = _behaviour.runSpeed;
			_behaviour.NavAgent.SetDestination(_behaviour.ItemToFetch.GameObject.transform.position);
			_mahleeThrowPosition = _behaviour.mahlee.position;
		}

		public void OnExit()
		{
			LetItemGo();
		}

		private void PickupItem()
		{
			Debug.Assert(_behaviour.ItemToFetch != null);

			var itemGobj = _behaviour.ItemToFetch.GameObject;

			_behaviour.ItemToFetch.IsInteractable = false;
			itemGobj.transform.position = _behaviour.mouth.position;
			itemGobj.transform.SetParent(_behaviour.mouth, true);

			var rb = itemGobj.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.isKinematic = true;
			}
		}

		private void LetItemGo()
		{
			if(_behaviour.ItemToFetch != null)
			{
				var itemGobj = _behaviour.ItemToFetch.GameObject;

				_behaviour.ItemToFetch.IsInteractable = true;
				itemGobj.transform.SetParent(null);
				var rb = itemGobj.GetComponent<Rigidbody>();
				if (rb)
				{
					rb.isKinematic = false;
				}
			}
		}
	}
}