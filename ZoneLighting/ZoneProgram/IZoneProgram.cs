namespace ZoneLighting.ZoneProgram
{
	public interface IZoneProgram
	{
		void Start();
		void Stop();
		Zone Zone { get; set; }
	}
}
