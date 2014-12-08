using System;
using System.Drawing;
using System.Threading;
using ZoneLighting;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			ZoneLightingManager.Instance.Initialize();
			System.Console.WriteLine(ZoneLightingManager.Instance.GetZoneSummary());

			Thread.Sleep(5000);

			ZoneLightingManager.Instance.Zones[0].ZoneProgram.SetInput("Color", Color.Red);

			Thread.Sleep(Timeout.Infinite);

			//ZoneLightingManager.Instance.Initialize();
			//Thread.Sleep(Timeout.Infinite);

			//FadeCandyController.Instance.Initialize();
			//FadeCandyController.Instance.SendPixelFrame(new OPCPixelFrame(0, new byte[]
			//{
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127,
			//	127, 127, 127
			//}));
		}
	}
}
