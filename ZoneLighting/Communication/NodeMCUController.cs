using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.Communication
{
    public class NodeMCUController : LightingController
    {
        public override void SendPixelFrame(IPixelFrame opcPixelFrame)
        {
            throw new NotImplementedException();
        }

        public override void SendLEDs(IList<ILightingControllerPixel> leds)
        {
            throw new NotImplementedException();
        }

        public override Type PixelType { get; }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
