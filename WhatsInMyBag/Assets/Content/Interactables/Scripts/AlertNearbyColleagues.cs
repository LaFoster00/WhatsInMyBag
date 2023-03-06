using System.Collections.Generic;
using System.Linq;
using Bolt;
using Content.NPC.Scripts;
using Ludiq;
using UnityEngine;

namespace Content.Interactables.Scripts
{
	public class AlertNearbyColleagues : Unit
	{
		[DoNotSerialize]
		public ControlInput input { get; private set; }

		[DoNotSerialize]
		public ValueInput alertInfo { get; private set; }

		[DoNotSerialize]
		public ValueInput rangeIn { get; private set; }

		[DoNotSerialize]
		public ValueInput numEnemiesIn { get; private set; }

		[DoNotSerialize]
		public ControlOutput outputFound { get; private set; }

		[DoNotSerialize]
		public ControlOutput outputNotFound { get; private set; }

		[DoNotSerialize]
		public ValueOutput colleagues { get; private set; }


		/// <summary>
		/// Gets the n closest coworkers in a sphere
		/// </summary>
		/// <param name="origin">midpoint of the sphere</param>
		/// <param name="range">radius of the sphere to look in</param>
		/// <param name="nColleagues">maximum number of coworkers we want to have returned</param>
		/// <returns>a list of the at maximum nColleagues closest coworkers that are less than range away from the origin</returns>
		public static List<AlertableNPC> AlertNearby(AlertionInfo alert, float range, int nColleagues = 1)
		{
			var alertOrigin = alert.alertionObject.transform.position;
			var colls = Physics.OverlapSphere(alertOrigin, range);

			if (colls.Length == 0)
				return new List<AlertableNPC>();

			var closeColleagues = colls.AsEnumerable().Where(col => col.CompareTag("Colleague"))
			                     .Select(col => (Vector3.Distance(col.transform.position, alertOrigin), col))
			                     .Where(tuple => tuple.Item1 < range) // filter distance
			                     .OrderBy(tuple => tuple.Item1) // sort by distance
			                     .Select(tuple => tuple.col.GetComponent<AlertableNPC>()) // select alertable npc
			                     .Where(npc => npc != null && npc.IsAlertedOn(alert.alertTypes))
			                     .ToList();
			
			// remove the farthest away colleagues if we have mor than nColleagues
			if (closeColleagues.Count >= nColleagues)
			{
				closeColleagues.RemoveRange(nColleagues, closeColleagues.Count - nColleagues);
			}

			foreach (AlertableNPC alertableNpc in closeColleagues)
			{
				alertableNpc.Alert(alert);
			}

			return closeColleagues;
		}

		protected override void Definition()
		{
			input = ControlInput("In", Enter);
			outputFound = ControlOutput("Found colleagues");
			outputNotFound = ControlOutput("Not found any");

			alertInfo = ValueInput<AlertionInfo>("Alert Info");
			rangeIn = ValueInput<float>("Range");
			numEnemiesIn = ValueInput<int>("Max num colleagues");

			colleagues = ValueOutput<List<AlertableNPC>>("Closest colleagues");

			Requirement(alertInfo, input);
			Requirement(rangeIn, input);
			Requirement(numEnemiesIn, input);
		}

		private ControlOutput Enter(Flow flow)
		{
			var alertingObj = flow.GetValue<AlertionInfo>(alertInfo);
			var range = flow.GetValue<float>(rangeIn);
			var numColleagues = flow.GetValue<int>(numEnemiesIn);
			var nearby = AlertNearby(alertingObj, range, numColleagues);
			flow.SetValue(colleagues, nearby);

			if (nearby.Count != 0)
			{
				return outputFound;
			}
			else
			{
				return outputNotFound;
			}
		}
	}
}
