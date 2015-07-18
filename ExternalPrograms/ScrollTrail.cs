using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Scrolls a trail across a length of Lights
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "ScrollTrail")]
	public class ScrollTrail : LoopingZoneProgram
	{
		#region Inputs

		int DelayTime { get; set; } = 65;
		float DarkenFactor { get; set; } = (float)0.8;

		/// <summary>
		/// Whether or not two or more trails share the same shape (pixels). Set this to true
		/// if there are trails following trails.
		/// </summary>
		bool ShareShape { get; set; }

		List<TrailShape> TrailShapes { get; set; } = new List<TrailShape>();

		#endregion

		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
			AddMappedInput<float>(this, "DarkenFactor");
			AddMappedInput<List<TrailShape>>(this, "TrailShapes");
			AddMappedInput<bool>(this, "ShareShape");
		}

		public override void Loop()
		{
			var sendColors = new Dictionary<int, Color>();

			TrailShapes.ForEach(trailShape =>
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
						sendColors[trailShape.Shape.Pixels[i]] = GetColor(trailShape.Shape.Pixels[i]).Darken(DarkenFactor);
					else
						//if not sharing the shape with another trailshape, then clean out the rest of the shape
						if (!ShareShape)
						sendColors[trailShape.Shape.Pixels[i]] = Color.Black;

					//set lead index color
					if (i == leadIndex)
						sendColors[leadPixel] = dotColor ?? ProgramCommon.GetRandomColor();
				}

				trailShape.Trail.LeadIndex = trailShape.Shape.GetNextIndex(leadIndex);
			});

			SendColors(sendColors);     //send frame
			ProgramCommon.Delay(DelayTime); //pause before next iteration
			SyncContext?.SignalAndWait(100);
		}

		protected override void PostStop(bool force)
		{
			////TODO:fade out lights
			//foreach (var logicalRGBLight in Zone.SortedLights)
			//{

			//}
		}
	}

	public class TrailShape
	{
		public TrailShape(Trail trail, Shape shape)
		{
			Trail = trail;
			Shape = shape;
		}

		public Trail Trail { get; }
		public Shape Shape { get; set; }
	}

	public class Trail
	{
		public Trail(int length, Color color)
		{
			Length = length;
			Color = color;
		}

		public int LeadIndex { get; set; } = 0;

		public int Length { get; }

		public Color? Color { get; }
	}

	public class Shape
	{
		public Shape(params int[] pixels)
		{
			Pixels = pixels;
		}

		public int[] Pixels { get; }

		public int GetNextIndex(int index) => index == Pixels.Length - 1 ? 0 : index + 1;
		public int GetPreviousIndex(int index, out bool overflow)
		{
			overflow = false;
			if (index == 0)
			{
				overflow = true;
				return Pixels.Length - 1;
			}
			else
				return index - 1;
		}

		public int GetNextPixel(int pixel) => Pixels.Last() == pixel ? Pixels.First() : Pixels[Array.IndexOf(Pixels, pixel) + 1];

		public int GetPreviousPixel(int pixel) => Pixels.First() == pixel ? Pixels.Last() : Pixels[Array.IndexOf(Pixels, pixel) - 1];

		public int PixelCount => Pixels.Length;
	}
}