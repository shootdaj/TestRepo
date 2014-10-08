﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting;
using ZoneLighting.Communication;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			//ZoneLightingManager.Instance.Initialize();
			//Thread.Sleep(Timeout.Infinite);

			FadeCandyController.Instance.Initialize();
			FadeCandyController.Instance.SendPixelFrame(new OPCPixelFrame(1, new byte[]
			{
				255, 255, 255,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0,
				0, 0, 0
			}));
		}
	}
}
