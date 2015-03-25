using ZoneLighting.ZoneNS;

namespace ZoneLighting.StockPrograms
{
	public interface IStepper
	{
		int CurrentStep { get; set; }
		int EndStep { get; set; }
		bool PauseForTest { get; set; }
		int StartStep { get; }
		SyncLevel SyncLevel { get; set; }

		void Loop();
		void Setup();
	}
}