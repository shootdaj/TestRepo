namespace ZoneLighting.Graphics.Drawing
{
	public class TrailShape
	{
		public TrailShape(Trail trail, Shape shape, float darkenFactor = (float)0.5)
		{
			Trail = trail;
			Shape = shape;
			DarkenFactor = darkenFactor;
		}

		public Trail Trail { get; }
		public Shape Shape { get; set; }
		public float DarkenFactor { get; set; } = (float)0.8;
	}
}