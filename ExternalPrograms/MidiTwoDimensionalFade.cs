using System.ComponentModel.Composition;
using System.Drawing;
using MIDIator.Engine;
using MIDIator.Interfaces;
using MIDIator.Services;
using Sanford.Multimedia.Midi;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Does MIDI magic.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "MidiTwoDimensionalFade")]
	public class MidiTwoDimensionalFade : ReactiveZoneProgram
	{
		private IMIDIInputDevice MidiInput { get; set; }

		protected override void StartCore(dynamic parameters = null)
		{
			var midiDeviceService = new MIDIDeviceService();
			var virtualMIDIManager = new VirtualMIDIManager();
			MIDIManager.Instantiate(midiDeviceService, new ProfileService(midiDeviceService, virtualMIDIManager, null),
				virtualMIDIManager);

			MidiInput = MIDIManager.Instance.MIDIDeviceService.GetInputDevice(parameters?.DeviceID);
			MidiInput.AddChannelMessageAction(new ChannelMessageAction(message => true, TwoDimensionalFade));
			MidiInput.Start();
		}


		private int X { get; set; }
		private int Y { get; set; }

		private void TwoDimensionalFade(ChannelMessage message)
		{
			//ProgramCommon.Blink(new List<Tuple<Color, int>>
			//{
			//	{Color.FromArgb(args.Message.Data1 * 10, 50, 50), 500},
			//	{Color.Empty, 200}
			//}, OutputColor, SyncContext);

			if (message.Data1 == (int)Axis.X)
			{
				X = message.Data2;
			}
			else if (message.Data1 == (int)Axis.Y)
			{
				Y = message.Data2;
			}

			SendColor(Color.FromArgb(X, Y, 50));
		}

		protected override void Setup()
		{
		}

		protected override void StopCore(bool force)
		{
			MidiInput.Stop();
			MidiInput = null;
		}

		public enum Axis
		{
			X = 9,
			Y = 10
		}
	}
}