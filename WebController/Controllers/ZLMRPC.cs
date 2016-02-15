using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using AustinHarris.JsonRpc;
using WebController.IoC;
using ZoneLighting;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	/// <summary>
	/// Encapsulates an RPC-style controller implementation for ZLM.
	/// </summary>
	public class ZLMRPC : JsonRpcService, IZLMRPC
	{
		#region Internals

		public ZLMRPC(IZLM zlm)
		{
			Construct(zlm);
		}

		public void Construct(IZLM zlm)
		{
			ZLM = zlm;
		}

		public IZLM ZLM { get; private set; }

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
		private IEnumerable<Zone> DetermineZones(List<string> zoneNames)
		{
			IEnumerable<Zone> zones;
			if (zoneNames.Count == 1 && zoneNames.First().ToUpperInvariant() == "ALL")
			{
				zones = ZLM.AvailableZones;
			}
			else
			{
				zones = ZLM.Zones.Where(zone => zoneNames.Contains(zone.Name));
			}
			return zones;
		}


		#endregion

		#region API

		#region Admin 

		/// <summary>
		/// Creates a ZoneLightingManager.
		/// </summary>
		[JsonRpcMethod]
		public void CreateZLM()
		{
			Container.CreateZLM();
			Construct(Container.ZLM);
		}

		[JsonRpcMethod]
		public void Save()
		{
			ZLMAction(zlm =>
			{
				zlm.SaveZones();
				zlm.SaveProgramSets();
			});
		}

		[JsonRpcMethod]
		public void DisposeZLM()
		{
			ZLMAction(zlm => zlm.Dispose());
		}

		#endregion

		#region Program Set

		[JsonRpcMethod]
		public void DisposeProgramSets()
		{
			ZLMAction(zlm => zlm.DisposeProgramSets());
		}

		[JsonRpcMethod()]
		public void StartProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv)
		{
			var zones = DetermineZones(zoneNames);

			ZLMAction(zlm =>
			{
				zlm.DisposeProgramSets(programSetName);
				zlm.CreateProgramSet(programSetName, programName, false, isv, zones);
			});
		}

		[JsonRpcMethod()]
		public void StopProgramSet(string programSetName)
		{
			ZLMAction(zlm => zlm.ProgramSets.First(z => z.Name == programSetName).Stop());
		}

		#endregion

		#region Zone API

		//public void StartZone(string zoneName)
		//{
		//	ZLMAction(zlm => zlm.Zones.First(z => z.Name == zoneName));
		//}

		[JsonRpcMethod]
		public void StopZone(string zoneName)
		{
			ZLMAction(zlm => zlm.Zones.First(z => z.Name == zoneName).Stop(true));
		}


		[JsonRpcMethod()]
		public List<string> GetZoneNames()
		{
			return ZLM.Zones.Select(zone => zone.Name).ToList();
		}

		[JsonRpcMethod()]
		public void SetInputs(string zoneName, ISV isv)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones[zoneName].ZoneProgram.SetInputs(isv);
			});
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
		public void SetZoneColor(string zoneName, string color, float brightness)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones[zoneName].SetColor(Color.FromName(color).Darken(brightness));
				zlm.Zones[zoneName].SendLights(zlm.Zones[zoneName].LightingController);
			});
		}

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
	}
}