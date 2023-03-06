using UnityEngine;

namespace Content.Common.Scripts
{
	[RequireComponent(typeof(Rigidbody))]
	public class SetCenterOfMass : MonoBehaviour
	{
		public Transform center;
		public Vector3   offset;

		private void Start()
		{
			if (!center)
				center = transform;
			
			GetComponent<Rigidbody>().centerOfMass = center.position + offset;
		}
	}
}