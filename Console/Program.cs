using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting;

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
						dynamic parameters = new ExpandoObject();
						parameters.Color = color;
						parameters.Time = 500;
						parameters.Soft = false;

						ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
						ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
					}
				}
			});

			task.Start();
			
			Thread.Sleep(Timeout.Infinite);

			//DebugTools.Print();
		}
	}
}
