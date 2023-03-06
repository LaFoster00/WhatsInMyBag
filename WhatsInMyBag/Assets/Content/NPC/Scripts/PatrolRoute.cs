using UnityEngine;

namespace Content.NPC.Scripts
{
	public class PatrolRoute : MonoBehaviour
	{
		public Transform[] Points { get; private set; }

		private void Start()
		{	
			Points = GetComponentsInChildren<Transform>();
			
			Debug.Assert(Points.Length >= 2, "Route must have at least 2 points");
		}
	}
}