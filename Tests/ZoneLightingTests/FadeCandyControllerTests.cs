using FakeItEasy;
using Xunit;
using ZoneLighting.Communication;

namespace ZoneLightingTests
{
	public class FadeCandyControllerTests
	{
		[Fact]
		public void PixelType_ReturnsIFadeCandyPixel()
		{
			var fadeCandyController = new FadeCandyController(A.Dummy<string>());
			Assert.Equal(fadeCandyController.PixelType, typeof(IFadeCandyPixel));
		}
	}
}
