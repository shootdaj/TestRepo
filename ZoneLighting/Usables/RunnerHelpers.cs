using System;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting.Usables
{
	public class RunnerHelpers
	{
		public static Action AddBasementZonesAndProgramsWithSync(ZLM zlm)
		{
			return () =>
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
			};
		}

		public static Action AddNeopixelZonesAndProgramsWithSync(ZLM zlm)
		{
			return () =>
			{
				var notificationSyncContext = new SyncContext("NotificationContext");

				//add zones
				var row12 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row12", PixelType.FadeCandyWS2812Pixel, 16, 1);
				var row34 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row34", PixelType.FadeCandyWS2812Pixel, 16, 2);
				var row56 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row56", PixelType.FadeCandyWS2812Pixel, 16, 3);
				var row78 = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "Row78", PixelType.FadeCandyWS2812Pixel, 16, 4);

				zlm.CreateProgramSet("RainbowSet", "Rainbow", true, null, zlm.Zones);

				//setup interrupting inputs - in the real code this method should not be used. The ZoneScaffolder.AddInterruptingProgram should be used.
				row12.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
				row34.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
				row56.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
				row78.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);

				//synchronize and start interrupting programs
				notificationSyncContext.Sync(row12.InterruptingPrograms[0],
					row34.InterruptingPrograms[0],
					row56.InterruptingPrograms[0],
					row78.InterruptingPrograms[0]);

				row12.InterruptingPrograms[0].Start();
				row34.InterruptingPrograms[0].Start();
				row56.InterruptingPrograms[0].Start();
				row78.InterruptingPrograms[0].Start();
			};
		}


		private static Action AddBasementZonesAndPrograms()
	    {
	        return () =>
	        {
	            //var notificationSyncContext = new SyncContext();

	            ////add zones
	            //var adsf = ZoneScaffolder.Instance.AddFadeCandyZone()

	            //var leftWing =  AddFadeCandyZone("LeftWing", PixelType.FadeCandyWS2812Pixel, 6, 1);
	            //var center = AddFadeCandyZone("Center", PixelType.FadeCandyWS2811Pixel, 21, 2);
	            //var rightWing = AddFadeCandyZone("RightWing", PixelType.FadeCandyWS2812Pixel, 12, 3);
	            //var baiClock = AddFadeCandyZone("BaiClock", PixelType.FadeCandyWS2812Pixel, 24, 4);

	            //var rainbowSet = new ProgramSet("Rainbow",)

	            ////initialize zones
	            //leftWing.Initialize(new Cylon(), null);//, true, syncContext, true);
	            //center.Initialize(new Cylon(), null);//, true, syncContext, true);
	            //rightWing.Initialize(new Cylon(), null);//, true, syncContext, true);
	            //baiClock.Initialize(new Cylon(), null);//, true, syncContext, true);

	            //////synchronize and start zone programs
	            ////syncContext.Sync(leftWing.ZoneProgram,
	            ////						center.ZoneProgram,
	            ////						rightWing.ZoneProgram,
	            ////						baiClock.ZoneProgram);

	            ////leftWing.ZoneProgram.Start();
	            ////rightWing.ZoneProgram.Start();
	            ////center.ZoneProgram.Start();
	            ////baiClock.ZoneProgram.Start();

	            ////setup interrupting inputs
	            //leftWing.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
	            //rightWing.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
	            //center.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
	            //baiClock.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);

	            ////synchronize and start interrupting programs
	            ////notificationSyncContext.SyncAndStart(leftWing.InterruptingPrograms[0],
	            ////									rightWing.InterruptingPrograms[0],
	            ////									center.InterruptingPrograms[0],
	            ////									baiClock.InterruptingPrograms[0]);

	            //leftWing.InterruptingPrograms[0].Start();
	            //rightWing.InterruptingPrograms[0].Start();
	            //center.InterruptingPrograms[0].Start();
	            //baiClock.InterruptingPrograms[0].Start();
	        };
	    }
	}
}
