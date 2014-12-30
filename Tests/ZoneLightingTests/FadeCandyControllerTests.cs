using FakeItEasy;
using NUnit.Framework;
using ZoneLighting.Communication;

namespace ZoneLightingTests
{
	public class FadeCandyControllerTests
	{
		[Test]
		public void PixelType_ReturnsIFadeCandyPixel()
		{
			var fadeCandyController = new FadeCandyController(A.Dummy<string>());
			Assert.AreEqual(typeof(IFadeCandyPixel), fadeCandyController.PixelType);
			//fadeCandyController.Dispose();
		}
	}
}
