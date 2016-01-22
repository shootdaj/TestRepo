using WebController.Controllers;
using ZoneLighting;

namespace WebController.ContainerNS
{
	public class Container
	{
		public static IZLM ZLM { get; set; }

		public static IZLMRPC ZLMRPC { get; set; }
	}
}