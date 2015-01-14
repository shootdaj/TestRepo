using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace Console
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ZoneLightingManager.Instance.Initialize(false, false);

			var task = new Task(() =>
			{
				while (true)
				{
					var input = System.Console.ReadLine();
					var color = Color.FromName(input);
					if (color.IsKnownColor)
					{
						//ZoneLightingManager.Instance.Zones[0].InterruptingPrograms[0].SetInput("Color", Color.Red);

						var blinkSyncContext = new SyncContext();

						//ZoneLightingManager.Instance.Zones[0].InterruptingPrograms[0].AttachBarrier(blinkSyncContext.Barrier);
      //                  ZoneLightingManager.Instance.Zones[1].InterruptingPrograms[0].AttachBarrier(blinkSyncContext.Barrier);
      //                  ZoneLightingManager.Instance.Zones[2].InterruptingPrograms[0].AttachBarrier(blinkSyncContext.Barrier);
      //                  ZoneLightingManager.Instance.Zones[3].InterruptingPrograms[0].AttachBarrier(blinkSyncContext.Barrier);


						DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						ZoneLightingManager.Instance.Zones[0].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						ZoneLightingManager.Instance.Zones[1].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						ZoneLightingManager.Instance.Zones[2].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						ZoneLightingManager.Instance.Zones[3].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");

						//DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						//ZoneLightingManager.Instance.Zones[0].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						//DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						//DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						//ZoneLightingManager.Instance.Zones[1].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						//DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						//DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						//ZoneLightingManager.Instance.Zones[2].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						//DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
						//DebugTools.AddEvent("Program.Main", "START Setting Interrupting Input");
						//ZoneLightingManager.Instance.Zones[3].InterruptingPrograms[0].SetInput("Blink", new Tuple<Color, int>(color, 500), blinkSyncContext.Barrier);
						//DebugTools.AddEvent("Program.Main", "END Setting Interrupting Input");
					}
				}
			});

			//task.Start();
			
			Thread.Sleep(Timeout.Infinite);

			//DebugTools.Print();
		}
	}
}
