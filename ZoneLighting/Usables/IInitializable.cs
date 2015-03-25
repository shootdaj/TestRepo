namespace ZoneLighting.Usables
{
	public interface IInitializable
	{
		void Initialize();
		bool Initialized { get; }
		void Uninitialize();
	}

	public interface IInitializableBool : IInitializable
	{
		new bool Initialize();
		new bool Uninitialize();
	}
}
