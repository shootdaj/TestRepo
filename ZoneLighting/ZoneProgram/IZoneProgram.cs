namespace ZoneLighting.ZoneProgram
{
	public interface IZoneProgram
	{
		void Start(IZoneProgramParameter parameter);
		void Stop();
		//void Pause();
		Zone Zone { get; set; }
	}
}
