using System;
using System.Collections.Generic;
using System.Dynamic;
using Refigure;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting.Usables
{
	public static class RunnerHelpers
	{
		public static Dictionary<int, int> LivingRoomLogicalPhysicalMapping = new Dictionary<int, int>
		{
			{0, 5},
			{1, 3},
			{2, 0},
			{3, 2},
			{4, 4},
			{5, 1}
		};

		public static Action AddBasementZonesAndProgramsWithSyncAction(ZLM zlm)
		{
			return () =>
			{
                AddBasementZonesAndProgramsWithSync(zlm);
			};
		}

		public static void AddBasementZonesAndProgramsWithSync(ZLM zlm)
		{
			var notificationSyncContext = new SyncContext("NotificationContext");

			//add zones
			var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel, 6,
				1);
			var center = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Center", PixelType.FadeCandyWS2811Pixel, 21, 2);
			var rightWing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "RightWing", PixelType.FadeCandyWS2812Pixel,
				12, 3);
			var baiClock = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "BaiClock", PixelType.FadeCandyWS2812Pixel, 24,
				4);

			zlm.CreateProgramSet("RainbowSet", "Rainbow", true, null, zlm.Zones);

			//setup interrupting inputs - in the real code this method should not be used. The ZoneScaffolder.AddInterruptingProgram should be used.
			leftWing.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			rightWing.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			center.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			baiClock.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);

			//synchronize and start interrupting programs
			notificationSyncContext.Sync(leftWing.InterruptingPrograms[0],
				rightWing.InterruptingPrograms[0],
				center.InterruptingPrograms[0],
				baiClock.InterruptingPrograms[0]);

			leftWing.InterruptingPrograms[0].Start();
			rightWing.InterruptingPrograms[0].Start();
			center.InterruptingPrograms[0].Start();
			baiClock.InterruptingPrograms[0].Start();
		}

		public static void AddNeopixelZonesAndProgramsWithSyncMethod(ZLM zlm)
		{
			var notificationSyncContext = new SyncContext("NotificationContext");

			//add zones
			//var row12 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row12", PixelType.FadeCandyWS2812Pixel, 16, 1, 0.5);
			//var row34 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row34", PixelType.FadeCandyWS2812Pixel, 16, 2, 0.5);
			//var row56 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row56", PixelType.FadeCandyWS2812Pixel, 16, 3, 0.5);
			//var row78 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row78", PixelType.FadeCandyWS2812Pixel, 16, 4, 0.5);

			var neoMatrix = CreateNeoMatrixZone(zlm);

			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 0.3);
			isv.Add("Random", true);

			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones);

			//setup interrupting inputs - in the real code this method should not be used. The ZoneScaffolder.AddInterruptingProgram should be used.
			neoMatrix.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			
			//synchronize and start interrupting programs
			notificationSyncContext.Sync(neoMatrix.InterruptingPrograms[0]);

			neoMatrix.InterruptingPrograms[0].Start();
		}

		public static Action AddNeopixelZonesAndProgramsWithSync(ZLM zlm)
		{
			return () =>
			{
				AddNeopixelZonesAndProgramsWithSyncMethod(zlm);
			};
		}

		public static Action RunShimmerOnNeoMatrixAction(ZLM zlm)
		{
			return () =>
			{
				RunShimmerOnNeoMatrix(zlm);
			};
		}

		public static void RunShimmerOnNeoMatrix(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 0.3);
			isv.Add("Random", true);
			//isv.Add("ColorScheme", ColorScheme.Primaries);

			dynamic startingParameters = new ExpandoObject();
			startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));

			CreateNeoMatrixZone(zlm);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones, startingParameters: startingParameters);
		}

		public static void RunShimmerOnNeoMatrixWithoutMIDI(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 0.3);
			isv.Add("Random", true);
			//isv.Add("ColorScheme", ColorScheme.Primaries);
			

			CreateNeoMatrixZone(zlm);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones, startingParameters: null);
		}

		public static void RunShimmerAndBlinkColorReactiveOnNeoMatrix(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 0.3);
			isv.Add("Random", true);
			//isv.Add("ColorScheme", ColorScheme.Primaries);

			dynamic startingParameters = new ExpandoObject();
			startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));

			var zone = CreateNeoMatrixZone(zlm);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones, startingParameters);
			zone.AddInterruptingProgram(new BlinkColorReactive());
			zone.InterruptingPrograms[0].Start();
		}

		public static void RunShimmerInLivingRoom(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 1.0);
			isv.Add("Random", true);
			//isv.Add("ColorScheme", ColorScheme.Primaries);

			dynamic startingParameters = new ExpandoObject();
			startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));

			var zone = CreateLivingRoomZone(zlm);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones, startingParameters);
			//zone.AddInterruptingProgram(new BlinkColorReactive());
			//zone.InterruptingPrograms[0].Start();
		}

		public static void RunMidiPlayInLivingRoom(ZLM zlm)
		{
			dynamic startingParameters = new ExpandoObject();
			startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));

			var zone = CreateLivingRoomZone(zlm);
			zlm.CreateProgramSet("MidiPlaySet", "LivingRoomMidiPlay", false, null, zlm.Zones, startingParameters);
		}

		public static void RunStopWatchBlinkInLivingRoom(ZLM zlm)
		{
			var zone = CreateLivingRoomZone(zlm);
			zlm.CreateProgramSet("StopWatchBlinkSet", "StopWatchBlink", false, null, zlm.Zones);
		}

		public static void RunMicroTimerClockBlinkOnNeoMatrix(ZLM zlm)
		{
			var zone = CreateNeoMatrixZone(zlm);
			zlm.CreateProgramSet("MicroTimerClockBlinkSet", "MicroTimerClockBlink", false, null, zlm.Zones);
		}

		public static void RunStopWatchBlinkOnNeoMatrix(ZLM zlm)
		{
			var zone = CreateNeoMatrixZone(zlm);
			zlm.CreateProgramSet("StopWatchBlinkSet", "StopWatchBlink", false, null, zlm.Zones);
		}

		public static Zone CreateNeoMatrixZone(ZLM zlm)
		{
			var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
				64, 1);

			return neomatrix;
		}

		public static Zone CreateLivingRoomZone(ZLM zlm)
		{
			const byte fadeCandyChannel = 1;
			const PixelType pixelType = PixelType.FadeCandyWS2812Pixel;

			var livingRoom = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "LivingRoom", pixelType, LivingRoomLogicalPhysicalMapping,
				fadeCandyChannel);
			
			return livingRoom;
		}

		public static void RunRainbowOnNeoMatrix(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("Speed", 10);
			isv.Add("DelayTime", 10);
			var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
				64, 1, 0.5);
			zlm.CreateProgramSet("RainbowSet", "Rainbow", false, isv, zlm.Zones);
		}

		public static void RunShimmerInBasement(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 1.0);
			isv.Add("Random", true);
			var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel,
				6, 1);
			var rightwing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "RightWing", PixelType.FadeCandyWS2812Pixel,
				12, 3);
			var center = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Center", PixelType.FadeCandyWS2811Pixel,
				21, 2);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones);
		}

		public static void RunRainbowInBasement(ZLM zlm)
		{
			var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel,
				6, 1);
			var rightwing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "RightWing", PixelType.FadeCandyWS2812Pixel,
				12, 3);
			zlm.CreateProgramSet("RainbowSet", "Rainbow", true, null, zlm.Zones);
		}

	    public static void RunMidiTwoDimensionalFadeOnNeoMatrix(ZLM zlm)
	    {
	        var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
	            64, 1);
	        dynamic startingParameters = new ExpandoObject();
	        startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));
	        zlm.CreateProgramSet("MidiTwoDimensionalFadeSet", "MidiTwoDimensionalFade", false, null, zlm.Zones,
	            startingParameters);
	    }

	    public static void RunMidiTwoDimensionalFadeInBasement(ZLM zlm)
	    {
            var isv = new ISV();
            var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel,
                6, 1);
            var rightwing = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "RightWing", PixelType.FadeCandyWS2812Pixel,
                12, 3);
            var center = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Center", PixelType.FadeCandyWS2811Pixel,
                21, 2);
			dynamic startingParameters = new ExpandoObject();
			startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID"));
		    zlm.CreateProgramSet("MidiTwoDimensionalFadeSet", "MidiTwoDimensionalFade", false, isv, zlm.Zones,
			    startingParameters);
	    }

		public static void RunShimmerOnNeoMatrixFourZones(ZLM zlm)
		{
			var isv = new ISV();
			isv.Add("MaxFadeSpeed", 1);
			isv.Add("MaxFadeDelay", 20);
			isv.Add("Density", 1.0);
			isv.Add("Brightness", 1.0);
			isv.Add("Random", true);
			var firstRow = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "FirstRow", PixelType.FadeCandyWS2812Pixel, 16, 1);
			var secondRow = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "SecondRow", PixelType.FadeCandyWS2812Pixel, 16, 2);
			var thirdRow = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "ThirdRow", PixelType.FadeCandyWS2812Pixel, 16, 3);
			var fourthRow = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "FourthRow", PixelType.FadeCandyWS2812Pixel, 16, 4);
			zlm.CreateProgramSet("ShimmerSet", "Shimmer", false, isv, zlm.Zones);
		}

        public static void RunVisualClockOnNeoMatrix(ZLM zlm)
        {
            CreateNeoMatrixZone(zlm);
            zlm.CreateProgramSet("VisualClockSet", "VisualClock", false, null, zlm.Zones);
        }
    }
}
