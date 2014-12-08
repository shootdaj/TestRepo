using System;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using Xunit;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneProgramInputTests
	{
		private void AddConsoleListener()
		{
			
		}

		[Fact]
		public void Value_DeserializesWithUnderlyingType()
		{
			Debug.Listeners.Add(new DefaultTraceListener());
			
			var dotColor = new ZoneProgramInput<Color>("DotColor", typeof(System.Drawing.Color));
			dotColor.Set(System.Drawing.Color.Red);

			var serializedDotColor = JsonConvert.SerializeObject(dotColor, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented,
			});

			var deserializedDotColor = JsonConvert.DeserializeObject<ZoneProgramInput<object>>(serializedDotColor);

			Console.WriteLine(deserializedDotColor.Value.GetType());

			Assert.True(deserializedDotColor.Value is Color);
		}

		//public void ZoneProgram_DeserializesWithUnderlyingTypeForInputsValue
		//{
			
		//}
	}
}
