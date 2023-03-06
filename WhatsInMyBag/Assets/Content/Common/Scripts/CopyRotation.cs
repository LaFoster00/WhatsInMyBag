using System;
using UnityEngine;

namespace Content.Common.Scripts
{
	public class CopyRotation : MonoBehaviour
	{
		public Transform target;

		private void Start()
		{
			if (!target)
			{
				var cam = Camera.main;
				target = cam.transform;
			}
		}

		[ExecuteAlways]
		private void LateUpdate()
		{
			if (target)
				transform.rotation = target.rotation;
		}
	}
}