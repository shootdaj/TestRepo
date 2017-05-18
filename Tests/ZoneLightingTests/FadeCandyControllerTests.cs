using System.Configuration;
using FakeItEasy;
using NUnit.Framework;
using WebSocketSharp;
using ZoneLighting.Communication;

namespace ZoneLightingTests
{
    public class FadeCandyControllerTests
    {
        [Test]
        public void PixelType_ReturnsIFadeCandyPixel()
        {
            var fadeCandyController = new FadeCandyController(A.Dummy<string>(), new DefaultPixelMapper(), 1);
            var result = fadeCandyController.OPCPixelType == OPCPixelType.OPCRGBPixel;
            fadeCandyController.Dispose();
            Assert.True(result);
        }

        [Test]
        public void Initialize_StartsFCServer()
        {
            var fadeCandyController = new FadeCandyController(ConfigurationManager.AppSettings["FadeCandyServerURL"], new DefaultPixelMapper(), 1);
            fadeCandyController.Initialize();
            var result = fadeCandyController.ConnectionState == WebSocketState.Open;
            fadeCandyController.Dispose();
            Assert.True(result);
        }
    }
}
