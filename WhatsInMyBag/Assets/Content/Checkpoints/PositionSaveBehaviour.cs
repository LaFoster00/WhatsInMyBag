using System.Collections.Generic;
using UnityEngine;

namespace Content.Checkpoints
{
	public class PositionSaveBehaviour : MonoBehaviour
	{
		public static Dictionary<string, GameObject> saveables = new Dictionary<string, GameObject>();

		private void Start()
		{
			if (transform.parent)
			{
				// makes this at least a tiny bit more robust as collisions only happen when objects have the same name & parent
				saveables[string.Concat(transform.parent.name, "/", name)] = gameObject;
			}
			else
			{
				saveables[name] = gameObject;
			}
		}
	}
}