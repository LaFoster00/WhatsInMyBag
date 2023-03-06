using System;
using Content.NPC.Scripts;
using MiscUtil.Extensions.TimeRelated;
using UnityEngine;
using UnityEngine.UI;

namespace Content.NPC.Coffee_Guy.Scripts
{
	[RequireComponent(typeof(NavigationWithArrival))]
	public class CoffeeGuyBehaviour : MonoBehaviour
	{
		[Tooltip("How long the guy stays at work")]
		public float workingDuration = 10f;
		[Tooltip("How long the guy stays at the coffe machine")]
		public float coffeeMakingDuration = 15f;
		[Tooltip("How long the guy stays at the place of alertion")]
		public float onAlertDuration = 3f;
		[Tooltip("How many seconds are between each run to the toilet")]
		public float peeingFrequency = 60f;
		[Tooltip("How long the guy stays in the toilet")]
		public float peeingDuration = 20f;


		public Transform workplace;
		public Transform coffeeMachine;
		[Tooltip("The toilet place (optional)")]
		public Transform toilets;
		public Slider progressBar;

		[Header("Face Changing")]
		public SkinnedMeshRenderer face;
		public Material neutral;
		public Material happy;
		public Material shocked;

		public static float MinDistanceToAlertion = 3.0f;
		

		public NavigationWithArrival Navigation { get; private set; }

		public ICoffeeGuyState CurrState { get; private set; }

		private void Start()
		{
			Debug.Assert(workplace, "Need to set a workplace for guy");
			Debug.Assert(coffeeMachine, "Need to set a coffee machine for guy");
			

			Navigation = GetComponent<NavigationWithArrival>();

			CurrState = new AlertableState();
			CurrState.Init(this);
			CurrState.OnEnter();
		}

		private void Update()
		{
			CheckStateTransition();

			CurrState.Update();
		}

		private void CheckStateTransition()
		{
			if (CurrState.Next() != null)
			{
				CurrState.OnExit();

				CurrState.Next().Init(this);
				CurrState = CurrState.Next();

				CurrState.OnEnter();
			}
		}

		public void DoAlert(AlertionInfo alertionInfo)
		{
			(CurrState as AlertableState)?.Alert(alertionInfo);
		}
	}
}