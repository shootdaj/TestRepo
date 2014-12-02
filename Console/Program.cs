using System.Threading;
using ZoneLighting;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			ZoneLightingManager.Instance.Initialize();
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
