using System;
using System.Linq;
using Refigure;
using WebController.Controllers;
using ZoneLighting;
using ZoneLighting.Usables;

namespace WebController.IoC
{
	public class Container
	{
		public static IZLM ZLM { get; set; }

		public static IZLMRPC ZLMRPC { get; set; }

		public static void CreateZLM()
		{	
			bool firstRun;
			if (!bool.TryParse(Config.Get("FirstRun"), out firstRun))
				firstRun = true;

			bool loadZoneModules;
			if (!bool.TryParse(Config.Get("LoadZoneModules"), out loadZoneModules))
				loadZoneModules = false;

			Action<ZLM> initAction = null;
			if (typeof (RunnerHelpers).GetMethods().Select(method => method.Name).Contains(Config.Get("InitAction")))
			{
				var initActionInfo = typeof (RunnerHelpers).GetMethods().First(method => method.Name == Config.Get("InitAction"));
				initAction = (Action<ZLM>) Delegate.CreateDelegate(typeof (Action<ZLM>), initActionInfo);
			}

			ZLM = new ZLM(loadZonesFromConfig: !firstRun,
				loadProgramSetsFromConfig: !firstRun,
				loadZoneModules: loadZoneModules, initAction: initAction);
		}

		public static void CreateZLMRPC()
		{
			if (ZLM == null)
				throw new Exception("Create ZLM first.");

			ZLMRPC = new ZLMRPC(ZLM);
		}
	}
}