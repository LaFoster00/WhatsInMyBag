using System.Collections.Generic;
using System.Linq;
using Bolt;
using Ludiq;
using UnityEngine;

namespace Content.Interactables.Scripts
{
	public class GetNearbyColleagues : Unit
	{
		[DoNotSerialize]
		public ControlInput input { get; private set; }

		[DoNotSerialize]
		public ValueInput originIn { get; private set; }

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
		public static List<GameObject> NearestColleague(Vector3 origin, float range, int nColleagues = 1)
		{
			var colls = Physics.OverlapSphere(origin, range);

			if (colls.Length == 0)
				return new List<GameObject>();

			var closeCols = colls.AsEnumerable().Where(col => col.CompareTag("Colleague"))
			                     .Select(col => (Vector3.Distance(col.transform.position, origin), col))
			                     .Where(tuple => tuple.Item1 < range) // filter distance
			                     .OrderBy(tuple => tuple.Item1) // sort by distance
			                     .Select(tuple => tuple.col.gameObject) // select game object
			                     .ToList();

			// remove the farthest away colleagues if we have mor than nColleagues
			if (closeCols.Count >= nColleagues)
			{
				closeCols.RemoveRange(nColleagues, closeCols.Count - nColleagues);
			}

			return closeCols;
		}

		protected override void Definition()
		{
			input = ControlInput("In", Enter);
			outputFound = ControlOutput("Found colleagues");
			outputNotFound = ControlOutput("Not found any");

			originIn = ValueInput<Vector3>("Origin");
			rangeIn = ValueInput<float>("Range");
			numEnemiesIn = ValueInput<int>("Max num colleagues");

			colleagues = ValueOutput<List<GameObject>>("Closest colleagues");

			Requirement(originIn, input);
			Requirement(rangeIn, input);
			Requirement(numEnemiesIn, input);
		}

		private ControlOutput Enter(Flow flow)
		{
			var origin = flow.GetValue<Vector3>(originIn);
			var range = flow.GetValue<float>(rangeIn);
			var numColleagues = flow.GetValue<int>(numEnemiesIn);
			var nearby = NearestColleague(origin, range, numColleagues);
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