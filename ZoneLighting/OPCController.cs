using System;
using ZoneLighting.Communication;

namespace ZoneLighting
{
    public abstract class OPCController : LightingController
    {
        public override Type PixelType => typeof(IOPCPixelContainer);
    }
}
