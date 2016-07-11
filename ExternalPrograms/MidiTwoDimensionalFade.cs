using System.ComponentModel.Composition;
using System.Drawing;
using MIDIator;
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
		private MIDIDevice MidiInput { get; set; }

		protected override void StartCore(dynamic parameters = null)
		{
			MidiInput = MIDIManager.GetDevice(parameters?.DeviceID);
			MidiInput.AddChannelMessageAction(TwoDimensionalFade);
			MidiInput.StartRecording();
		}

		private int X { get; set; }
		private int Y { get; set; }

		private void TwoDimensionalFade(object sender, ChannelMessageEventArgs args)
		{
			//ProgramCommon.Blink(new List<Tuple<Color, int>>
			//{
			//	{Color.FromArgb(args.Message.Data1 * 10, 50, 50), 500},
			//	{Color.Empty, 200}
			//}, OutputColor, SyncContext);

			if (args.Message.Data1 == (int)Axis.X)
			{
				X = args.Message.Data2;
			}
			else if (args.Message.Data1 == (int)Axis.Y)
			{
				Y = args.Message.Data2;
			}

			SendColor(Color.FromArgb(X, Y, 50));
		}

		protected override void Setup()
		{
		}

		protected override void StopCore(bool force)
		{
			MidiInput.StopRecording();
			MIDIManager.RemoveDevice(MidiInput);
		}

		public enum Axis
		{
			X = 9,
			Y = 10
		}
	}
}