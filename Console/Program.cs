using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			ZoneLightingManager.Instance.Initialize();

			Task.Run(() =>
			{
				while (true)
				{
					var input = System.Console.ReadLine();
					var color = Color.FromName(input);
					if (color.IsKnownColor)
					{
						ZoneLightingManager.Instance.Zones[0].InterruptingPrograms[0].SetInput("Color", color);
					}
				}
			});

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
