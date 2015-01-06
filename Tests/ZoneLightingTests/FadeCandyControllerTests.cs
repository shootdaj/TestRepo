using FakeItEasy;
using NUnit.Framework;
using ZoneLighting.Communication;

namespace ZoneLightingTests
{
	public class FadeCandyControllerTests
	{
		//[Fact]
		[Test]
		public void PixelType_ReturnsIFadeCandyPixel()
		{
			var fadeCandyController = new FadeCandyController(A.Dummy<string>());
			Assert.AreEqual(fadeCandyController.PixelType, typeof(IFadeCandyPixelContainer));
		}
	}
}
