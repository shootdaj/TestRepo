using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBash;
using ZoneLighting;

namespace WebController.NetBashCommands
{
	[WebCommand("stop", "Stops all programs in all zones.")]
	public class StopCommand : IWebCommand
	{
		public bool ReturnHtml
		{
			get { return false; }
		}

		public string Process(string[] args)
		{
			ZoneLightingManager.Instance.Uninitialize();
			return "Success";
		}
	}
}