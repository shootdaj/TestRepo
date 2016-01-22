using System;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using AustinHarris.JsonRpc;
using WebController.ContainerNS;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneProgramNS;
using Config = Refigure.Config;

namespace WebController.Controllers
{
	/// <summary>
	/// Encapsulates an RPC-style controller implementation for ZLM.
	/// </summary>
	public class ZLMRPC : JsonRpcService, IZLMRPC
	{
		public ZLMRPC(IZLM zlm)
		{
			ZLM = zlm;
		}

		public static IZLM ZLM { get; private set; }

		[JsonRpcMethod()]
		public static void CreateZLM()
		{	
			bool firstRun;
			if (!Boolean.TryParse(Config.Get("FirstRun"), out firstRun))
				firstRun = true;

			bool loadZoneModules;
			if (!Boolean.TryParse(Config.Get("LoadZoneModules"), out loadZoneModules))
				loadZoneModules = false;

			Action<ZLM> initAction = null;
			if (typeof (RunnerHelpers).GetMethods().Select(method => method.Name).Contains(Config.Get("InitAction")))
			{
				var initActionInfo = typeof (RunnerHelpers).GetMethods().First(method => method.Name == Config.Get("InitAction"));
				initAction = (Action<ZLM>) Delegate.CreateDelegate(typeof (Action<ZLM>), initActionInfo);
			}

			Container.ZLM = new ZLM(loadZonesFromConfig: !firstRun,
				loadProgramSetsFromConfig: !firstRun,
				loadZoneModules: loadZoneModules, initAction: initAction);
		}

		public static void ZLMAction(Action<IZLM> action)
		{
			action.Invoke(ZLM);
		}

		[JsonRpcMethod()]
		public void Save()
		{
			ZLMAction(zlm =>
			{
				zlm.SaveZones();
				zlm.SaveProgramSets();
			});
		}

		[JsonRpcMethod()]
		public void StopZone(string zoneName)
		{
			ZLMAction(zlm => { zlm.Zones.First(z => z.Name == zoneName).Stop(true); });
		}

		[JsonRpcMethod()]
		public void DisposeZLM()
		{
			ZLMController.ZLM.Dispose();
		}

		[JsonRpcMethod()]
		public void ProcessZLMCommand(string command, string programSetName, string programName)
		{
			if (command.ToLower().Trim() == "start")
			{
				ZLMAction(zlm =>
				{
					zlm.DisposeProgramSets(programSetName);

					var isv = new ISV();
					isv.Add("MaxFadeSpeed", 1);
					isv.Add("MaxFadeDelay", 20);
					isv.Add("Density", 1.0);
					isv.Add("Brightness", 1.0);
					isv.Add("Random", true);

					zlm.CreateProgramSet(programSetName, programName, false, isv, zlm.Zones);
				});
			}
			else if (command.ToLower().Trim() == "stop")
			{
				ZLMAction(zlm => zlm.ProgramSets.First(z => z.Name == programSetName).Stop());
			}
		}

		[JsonRpcMethod()]
		public void SetZoneColor(string zoneName, string color, float brightness)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones[zoneName].SetColor(Color.FromName(color).Darken(brightness));
				zlm.Zones[zoneName].SendLights(zlm.Zones[zoneName].LightingController);
			});
		}

		[JsonRpcMethod()]
		public void Notify(string colorString, int? time, int? cycles)
		{
			var color = Color.FromName(colorString);
			if (color.IsKnownColor)
			{
				dynamic parameters = new ExpandoObject();
				parameters.Color = color;
				parameters.Time = time;
				parameters.Soft = true;

				for (int i = 0; i < cycles; i++)
				{
					ZLMAction(
						zlm => { zlm.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters)); });
				}
			}
		}
	}
}