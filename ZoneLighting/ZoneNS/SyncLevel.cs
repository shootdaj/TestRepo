namespace ZoneLighting.ZoneNS
{
	public class SyncLevel
	{
		/// <summary>
		/// Mostly for diagnostic purposes.
		/// </summary>
		public string Name { get; set; }

		public SyncLevel(string name)
		{
			Name = name;
		}

		public static SyncLevel None = new SyncLevel("None");
	}
}
