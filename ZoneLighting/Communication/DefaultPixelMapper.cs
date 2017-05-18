﻿using System;

namespace ZoneLighting.Communication
{
    public class DefaultPixelMapper : IPixelToOPCPixelMapper
    {
        public int GetOPCPixelIndex(int pixelIndex)
        {
            return pixelIndex;
        }

	    public byte GetOPCPixelChannel(IPixel pixel)
	    {
			//this is where per-pixel mapping of the channel would happen
		    throw new NotImplementedException();
	    }
    }
}