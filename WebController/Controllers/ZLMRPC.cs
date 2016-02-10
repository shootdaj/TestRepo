using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebController.IoC;
using ZoneLighting;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	/// <summary>
	/// Encapsulates an RPC-style controller implementation for ZLM.
	/// </summary>
	public class ZLMRPC : JsonRpcService, IZLMRPC
	{
		public ZLMRPC(IZLM zlm)
		{
			Construct(zlm);
		}

		public void Construct(IZLM zlm)
		{
			ZLM = zlm;
		}

		public static IZLM ZLM { get; private set; }

		[JsonRpcMethod]
		public void CreateZLM()
		{
			Container.CreateZLM();
			Construct(Container.ZLM);
		}

		public static void ZLMAction(Action<IZLM> action)
		{
			action.Invoke(ZLM);
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
		public void StopZone(string zoneName)
		{
			ZLMAction(zlm => { zlm.Zones.First(z => z.Name == zoneName).Stop(true); });
		}

		[JsonRpcMethod]
		public void DisposeZLM()
		{
			ZLM.Dispose();
		}

		[JsonRpcMethod()]
		public void ProcessZLMCommand(string command, string programSetName, string programName, ISV isv)
		{
			if (command.ToLower().Trim() == "start")
			{
				ZLMAction(zlm =>
				{
					zlm.DisposeProgramSets(programSetName);
					zlm.CreateProgramSet(programSetName, programName, false, isv, zlm.Zones);
				});
			}
			else if (command.ToLower().Trim() == "stop")
			{
				ZLMAction(zlm => zlm.ProgramSets.First(z => z.Name == programSetName).Stop());
			}
		}

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

		//[JsonRpcMethod]
		//public void SetInput(string zoneName, string name, string data)
		//{
		//	ZLMAction(zlm =>
		//	{
		//		var realData = JsonConvert.DeserializeObject<typeisneededforconvertertobeused>(data, new JsonSerializerSettings()
		//		{
		//			Converters = new List<JsonConverter> { new Int32Converter() }
		//		});
		//		zlm.Zones[zoneName].ZoneProgram.SetInput(name, realData);
		//	});
		//}

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
	}
}