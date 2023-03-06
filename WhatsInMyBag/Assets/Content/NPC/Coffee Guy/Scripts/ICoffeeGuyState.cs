using System;

namespace Content.NPC.Coffee_Guy.Scripts
{
	public interface ICoffeeGuyState
	{
		public void Init(CoffeeGuyBehaviour guy);
		public void Update();
		public ICoffeeGuyState Next();

		public void OnEnter();
		public void OnExit();
	}
}