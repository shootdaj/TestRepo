using System;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting.Usables
{
	public class RunnerHelpers
	{
		public static Action AddBasementZonesAndProgramsWithSync()
		{
			return () =>
			{
				var notificationSyncContext = new SyncContext("NotificationContext");



				//add zones
				var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(ZLM.I.Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel, 6,
					1);
				var center = ZoneScaffolder.Instance.AddFadeCandyZone(ZLM.I.Zones, "Center", PixelType.FadeCandyWS2811Pixel, 21, 2);
				var rightWing = ZoneScaffolder.Instance.AddFadeCandyZone(ZLM.I.Zones, "RightWing", PixelType.FadeCandyWS2812Pixel,
					12, 3);
				var baiClock = ZoneScaffolder.Instance.AddFadeCandyZone(ZLM.I.Zones, "BaiClock", PixelType.FadeCandyWS2812Pixel, 24,
					4);

				ZLM.I.CreateProgramSet("RainbowSet", "Rainbow", true, null, ZLM.I.Zones);

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
