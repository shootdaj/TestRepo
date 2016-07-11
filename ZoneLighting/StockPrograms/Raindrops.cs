using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.Graphics.Drawing;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLighting.StockPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Raindrops")]
	public class Raindrops : ReactiveZoneProgram
	{
		private MicroClock LoopClock { get; set; }

		public List<ClockedTrailShape> ClockedTrailShapes { get; set; } = new List<ClockedTrailShape>();

		public Dictionary<int, Color> ColorsToSend { get; set; } = new Dictionary<int, Color>();

		public Raindrops()
		{
			LoopClock = new MicroClock(10000 //refresh interval
				, args => SendColors(ColorsToSend)
				, 500); //500us drift threshold
		}

		private void SetTrailColors(TrailShape trailShape)
		{
			var leadIndex = trailShape.Trail.LeadIndex;
			var leadPixel = trailShape.Shape.Pixels[leadIndex];
			var trailEnd = leadIndex; //this changes in the for loop after these declarations
			var dotColor = trailShape.Trail.Color;
			var overflow = false;

			//establish last index of trail
			for (var j = 0; j < trailShape.Trail.Length; j++)
			{
				bool tempOverflow;
				trailEnd = trailShape.Shape.GetPreviousIndex(trailEnd, out tempOverflow);
				if (tempOverflow)
					overflow = true;
			}

			for (var i = 0; i < trailShape.Shape.PixelCount; i++)
			{
				//darken pixels inside trail, blacken out the rest
				if (overflow ? (i < leadIndex || i >= trailEnd) : i < leadIndex && i >= trailEnd)
					ColorsToSend[trailShape.Shape.Pixels[i]] = GetColor(trailShape.Shape.Pixels[i]).Darken(trailShape.DarkenFactor);
				else
				//if not sharing the shape with another trailshape, then clean out the rest of the shape
				if (!false) //was if (!ShareShape)
					ColorsToSend[trailShape.Shape.Pixels[i]] = Color.Black;

				//set lead index color
				if (i == leadIndex)
					ColorsToSend[leadPixel] = dotColor ?? ProgramCommon.GetRandomColor();
			}

			trailShape.Trail.LeadIndex = trailShape.Shape.GetNextIndex(leadIndex);

			//Console.WriteLine(trailShape.ToString());
		}

		protected override void StartCore(dynamic parameters = null)
		{
			if (parameters == null || parameters.TrailShapes == null)
				throw new Exception("Parameter TrailShapes is required.");

			((List<TrailShape>)parameters.TrailShapes).ForEach(trailShape =>
			{
				var clockedTrailShape = new ClockedTrailShape();
				clockedTrailShape.Clock =
					new MicroClock(100000,
						args => SetTrailColors(clockedTrailShape.TrailShape), 500);
				clockedTrailShape.TrailShape = trailShape;
				ClockedTrailShapes.Add(clockedTrailShape);
			});

			LoopClock.Start();
			ClockedTrailShapes.ForEach(cts => cts.Start());
			base.StartCore(null);
		}

		protected override void StopCore(bool force)
		{
			ClockedTrailShapes.ForEach(cts => cts.Stop());
			LoopClock.Stop();
			base.StopCore(force);
		}

		#region Inputs

		public long LoopInterval
		{
			get { return LoopClock.Interval; }
			set { LoopClock.Interval = value; }
		}

		public long LoopDriftThreshold
		{
			get { return LoopClock.IgnoreEventIfLateBy; }
			set { LoopClock.IgnoreEventIfLateBy = value; }
		}

		public int NumberOfDrops => ClockedTrailShapes.Count;

		//private Color _color = Color.Red;

		//private ColorScheme _colorScheme = ColorScheme.All;

		//public bool UseColor { get; set; } = true;

		//public ColorScheme ColorScheme
		//{
		//	get { return _colorScheme; }
		//	set
		//	{
		//		UseColor = false;
		//		_colorScheme = value;
		//	}
		//}

		//public Color Color
		//{
		//	get { return _color; }
		//	set
		//	{
		//		UseColor = true;
		//		_color = value;
		//	}
		//}

		#endregion
	}
}
