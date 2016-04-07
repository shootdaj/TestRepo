using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using AustinHarris.JsonRpc;
using WebController.Extensions;
using WebController.IoC;
using WebController.Models;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	/// <summary>
	/// Encapsulates an RPC-style controller implementation for ZLM.
	/// </summary>
	public class ZLMRPC : JsonRpcService, IZLMRPC, IDisposable
	{
		#region Internals

		public ZLMRPC(IZLM zlm)
		{
			Construct(zlm);
		}

		private void Construct(IZLM zlm)
		{
			ZLM = zlm;
			Container.CreateAutomapConfig();
		}

		private IZLM ZLM { get; set; }

		#endregion

		#region Helpers

		/// <summary>
		/// Is used to ensure synchronous access to ZLM because ZLM is not designed for multithreaded access.
		/// </summary>
		/// <param name="action"></param>
		private void ZLMAction(Action<IZLM> action)
		{
			action.Invoke(ZLM);
		}

		#endregion

		#region API

		#region Admin 

		/// <summary>
		/// Adds a FadeCandy zone to ZLM. This 
		/// </summary>
		/// <param name="name">Name of zone</param>
		/// <param name="pixelType">Type of the pixel to use for the zone</param>
		/// <param name="numberOfLights">The number of lights.</param>
		/// <param name="channel">The channel.</param>
		/// <returns>The instance of the zone that was added.</returns>
		[JsonRpcMethod]
		public ZoneJsonModel AddFadeCandyZone(string name, PixelType pixelType, int numberOfLights, byte? channel)
		{
			return ZLM.AddFadeCandyZone(name, pixelType, numberOfLights, channel).ToJsonModel<Zone, ZoneJsonModel>();
		}

		/// <summary>
		/// Creates a ZoneLightingManager. This method is intended to be used on re-creations rather than the initial create.
		/// For initial creation of ZLM, use Container.CreateZLM().
		/// </summary>
		[JsonRpcMethod]
		public void CreateZLM()
		{
			Container.CreateZLM();
			Construct(Container.ZLM);
		}

		[JsonRpcMethod]
		public void DisposeZLM()
		{
			ZLMAction(zlm => zlm.Dispose());
		}

		[JsonRpcMethod]
		public void Save()
		{
			ZLMAction(zlm => zlm.Save());
		}

		[JsonRpcMethod]
		public string GetStatus()
		{
			var sb = new StringBuilder();
			sb.AppendLine("--ZoneLighting Summary--");
			sb.AppendLine("=============================");
			sb.AppendLine($"{ZLM.ProgramSets.Count} ProgramSet(s) currently running:");
			ZLM.ProgramSets.ForEach(ps =>
			{
				sb.AppendLine(
					$"{ps.Name} running {ps.ProgramName} on zone(s) {ps.Zones.Select(zone => zone.Name).Aggregate((i, j) => $"{i}, {j}")}" +
					(ps.Sync ? " in sync" : string.Empty));
			});
			sb.AppendLine("--End of Summary--");
			return sb.ToString();
		}

		#endregion

		#region Program Set

		[JsonRpcMethod]
		public ProgramSetJsonModel CreateProgramSet(string programSetName, string programName, IEnumerable<string> zoneNames, bool sync = true,
			ISV isv = null, dynamic startingParameters = null)
		{
			var model = new ProgramSetJsonModel();
			ProgramSet programSet = null;

			ZLMAction(
				zlm =>
					programSet =
						zlm.CreateProgramSet(programSetName, programName, zoneNames, sync, isv, startingParameters)
				);

			model = programSet.ToJsonModel<ProgramSet, ProgramSetJsonModel>();
			return model;
		}

		[JsonRpcMethod]
		public void DisposeProgramSet(string programSetName)
		{
			ZLMAction(zlm => zlm.DisposeProgramSets(programSetName.Listify()));
		}

		[JsonRpcMethod]
		public void DisposeProgramSets()
		{
			ZLMAction(zlm => zlm.DisposeProgramSets());
		}

		[JsonRpcMethod]
		public void RecreateProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv)
		{
			ZLMAction(zlm => zlm.RecreateProgramSet(programSetName, programName, zoneNames, isv));
		}

		[JsonRpcMethod]
		public void StartProgramSet(string programSetName)
		{
			ZLMAction(zlm => zlm.ProgramSets[programSetName].Start());
		}

		[JsonRpcMethod]
		public void StopProgramSet(string programSetName)
		{
			ZLMAction(zlm => zlm.ProgramSets[programSetName].Stop());
		}


		[JsonRpcMethod]
		public void SetProgramSetInputs(string programSetName, ISV isv)
		{
			ZLM.SetProgramSetInputs(programSetName, isv);
		}

		[JsonRpcMethod]
		public void RecreateProgramSetWithoutZone(string programSetName, string zoneName, bool force = false)
		{
			ZLM.RecreateProgramSetWithoutZone(programSetName, zoneName, force);
		}

		#endregion

		#region Zone

		//public void StartZone(string zoneName)
		//{
		//	ZLMAction(zlm => zlm.Zones.First(z => z.Name == zoneName));
		//}

		[JsonRpcMethod]
		public void StopZone(string zoneName, bool force)
		{
			ZLMAction(zlm => zlm.StopZone(zoneName, force));
		}

		[JsonRpcMethod]
		public List<ZoneJsonModel> GetZones()
		{
			//return ZLM.Zones.Select(zone => zone.Name).ToList();
			return ZLM.Zones.Select(zone => zone.ToJsonModel<Zone, ZoneJsonModel>()).ToList();
		}

		//[JsonRpcMethod]
		//public List<Zone> GetAvailableZones()
		//{
		//	return ZLM.AvailableZones;
		//}

		//public List<string> GetAvailableZoneNames()
		//{
			
		//}

		[JsonRpcMethod]
		public void SetZoneInputs(string zoneName, ISV isv)
		{
			ZLM.SetZoneInputs(zoneName, isv);
		}

		[JsonRpcMethod]
		public void SetZoneColor(string zoneName, string color, float brightness = 1)
		{
			ZLM.SetZoneColor(zoneName, color, brightness);
		}

		[JsonRpcMethod]
		public void SetLightColor(string zoneName, string color, int index, float brightness = 1)
		{
			ZLM.SetLightColor(zoneName, color, index, brightness);
		}

		[JsonRpcMethod]
		public void SetAllZonesColor(string color, float brightness = 1)
		{
			ZLM.SetAllZonesColor(color, brightness);
		}

		[JsonRpcMethod]
		public string GetZoneSummary()
		{
			var result = string.Empty;

			ZLMAction(zlm =>
			{
				result = zlm.GetZoneSummary();
			});

			return result;
		}

		#endregion

		#region Misc

		[JsonRpcMethod]
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

		#endregion
		
		///// <summary>
		///// Used to process a Command.
		///// </summary>
		///// <param name="commandName">Command to process.</param>
		///// <param name="parameters">Parameters for the command.</param>
		//[JsonRpcMethod()]
		//public void ProcessCommand(string commandName, dynamic parameters)
		//{
		//	var commandCandidates = Helper.GetAllAssignableTypes<Command>().Where(type => type.Name == commandName).ToList();
		//	if (!commandCandidates.Any())
		//	{
		//		throw new Exception("Invalid command.");
		//	}
		//	else
		//	{
		//		var command = (ICommand)Activator.CreateInstance(commandCandidates.First());
		//		ZLMAction(zlm => { command.Execute(zlm, parameters); });
		//	}


		//	//if (commandName.ToLower().Trim() == "start")
		//	//{
		//	//	ZLMAction(zlm =>
		//	//	{
		//	//		zlm.DisposeProgramSets(programSetName);
		//	//		zlm.CreateProgramSet(programSetName, programName, false, isv, zlm.Zones);
		//	//	});
		//	//}
		//	//else if (commandName.ToLower().Trim() == "stop")
		//	//{
		//	//	ZLMAction(zlm => zlm.ProgramSets.First(z => z.Name == programSetName).Stop());
		//	//}
		//}
		
		#endregion

		
		public void Dispose()
		{
			ZLM?.Dispose();
			base.Dispose();
		}
	}
}