using System;
using System.Linq;
using AutoMapper;
using Refigure;
using WebRemote.Automapper;
using WebRemote.Models;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.IoC
{
	public static class Container
	{
		public static IZLM ZLM { get; private set; }

		public static IZLMRPC ZLMRPC { get; private set; }

		public static MapperConfiguration MapperConfiguration { get; private set; }

		public static IMapper AutoMapper { get; private set; }

		public static void CreateZLM()
		{
			bool firstRun;
			if (!bool.TryParse(Config.Get("FirstRun"), out firstRun))
				firstRun = true;

			bool loadZoneModules;
			if (!bool.TryParse(Config.Get("LoadZoneModules"), out loadZoneModules))
				loadZoneModules = false;

			//get initAction from config
			Action<ZLM> initAction = null;
			if (typeof(RunnerHelpers).GetMethods().Select(method => method.Name).Contains(Config.Get("InitAction")))
			{
				var initActionInfo = typeof(RunnerHelpers).GetMethods().First(method => method.Name == Config.Get("InitAction"));
				initAction = (Action<ZLM>)Delegate.CreateDelegate(typeof(Action<ZLM>), initActionInfo);
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

		public static void CreateAutomapConfig()
		{
			MapperConfiguration = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<Zone, ZoneJsonModel>().ForMember(x => x.Inputs, opt => opt.ResolveUsing<ZoneJsonModelInputsResolver>());
					cfg.CreateMap<ProgramSet, ProgramSetJsonModel>();
					cfg.CreateMap<ZLM, ZLMJsonModel>();
				});
			MapperConfiguration.AssertConfigurationIsValid();
			AutoMapper = MapperConfiguration.CreateMapper();
		}
	}
}