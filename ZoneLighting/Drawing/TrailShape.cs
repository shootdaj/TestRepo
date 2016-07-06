namespace ZoneLighting.Drawing
{
	public class TrailShape
	{
		public TrailShape(Trail trail, Shape shape)
		{
			Trail = trail;
			Shape = shape;
		}

		public Trail Trail { get; }
		public Shape Shape { get; set; }
		public float DarkenFactor { get; set; } = (float)0.8;
	}
}