namespace Content.NPC.Doggo.Scripts
{
	public interface IDoggoState
	{
		public void Init(DoggoBehaviour guy);
		public void Update();
		public IDoggoState Next();
		public string Name();
		public void OnEnter();
		public void OnExit();
	}
}