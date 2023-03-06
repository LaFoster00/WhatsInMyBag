using UnityEngine;

namespace Content.NPC.IT_Lady.Scripts
{
	/// <summary>
	/// Every object that is breakable and should trigger the IT lady to come fix it need to have this script attached
	/// </summary>
	public class ITMachine : MonoBehaviour
	{
		public float minVelocityToBreak = 3.0f;

		public bool IsBroken { get; set; }

		private void OnCollisionEnter(Collision other)
		{
			if(IsBroken)
				return;


			if (minVelocityToBreak < other.relativeVelocity.magnitude)
			{
				Break();
			}
		}

		public void Repair()
		{
			IsBroken = false;
		}

		public void Break()
		{
			IsBroken = true;
			ItLadyAI.OnBrokeMachineDel.Invoke(this);
		}
	}
}