using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using MIDIator;
using Sanford.Multimedia.Midi;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "LivingRoomMidiPlay")]
	public class LivingRoomMidiPlay : ReactiveZoneProgram
	{
		private Dictionary<int, Color> ColorsToSend { get; set; }

		private MIDIDevice MidiInput { get; set; }

		private bool Random { get; set; } = false;

		protected override void StartCore(dynamic parameters = null, bool forceStoppable = true)
		{
			ForceStoppable = forceStoppable;
			MidiInput = MIDIManager.GetDevice(parameters?.DeviceID);
			MidiInput.AddChannelMessageAction(HandleMidi);
			MidiInput.StartRecording();

			ColorsToSend = new Dictionary<int, Color>();
			Zone.SortedLights.Keys.ToList().ForEach(lightIndex => ColorsToSend.Add(lightIndex, Color.Black));
		}

		private void HandleMidi(object sender, ChannelMessageEventArgs args)
		{
			var values = GetLightPositionAndColor(args);

			if (args.Message.Data2 == 0)
			{
				TurnOffLight(values.LightPosition);
			}
			else
			{
				TurnOnLight(values.LightPosition, values.LightColor);
			}
		}

		private dynamic GetLightPositionAndColor(ChannelMessageEventArgs args)
		{
			dynamic returnValue = new ExpandoObject();

			var adjustedGridStart = args.Message.Data1 - 0x24; //0x24 is the value of the top left button on the Numark Orbit and each subsequent button goes up by one horizontally
			returnValue.LightPosition = adjustedGridStart % 4;
			returnValue.LightColor = Color.Black;

			var colorBand = adjustedGridStart / 4;

			if (adjustedGridStart < 12)
			{
				switch (colorBand)
				{
					case 0:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Reds) : Color.Red;
						break;
					case 1:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Blues) : Color.Blue;
						break;
					case 2:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Greens) : Color.Green;
						break;
				}
			}
			else if (adjustedGridStart >= 12)
			{
				switch (adjustedGridStart)
				{
					case 12:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Primaries) : Color.Purple;
						returnValue.LightPosition = 4;
						break;
					case 13:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Secondaries) : Color.Yellow;
						returnValue.LightPosition = 4;
						break;
					case 14:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Primaries) : Color.Purple;
						returnValue.LightPosition = 5;
						break;
					case 15:
						returnValue.LightColor = Random ? ColorScheme.GetRandomSchemeColor(ColorScheme.Secondaries) : Color.Yellow;
						returnValue.LightPosition = 5;
						break;
				}
			}

			return returnValue;
		}

		private void TurnOnLight(int position, Color color)
		{
			ColorsToSend[position] = color;
			SendColors(ColorsToSend);
		}

		private void TurnOffLight(int position)
		{
			ColorsToSend[position] = Color.Black;
			SendColors(ColorsToSend);
		}

		protected override void SetupInterruptingInputs()
		{

		}

		protected override void StopCore(bool force)
		{

		}
	}
}
