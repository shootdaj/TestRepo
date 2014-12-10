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

			//Thread.Sleep(5000);

			//ZoneLightingManager.Instance.Zones[0].ZoneProgram.SetInput("Color", Color.Red);

			//Thread.Sleep(Timeout.Infinite);
		}
	}
}
