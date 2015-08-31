using System;
using System.ComponentModel.Composition;
using System.Drawing;
using Sanford.Multimedia.Midi;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	/// <summary>
	/// Does MIDI magic.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "MidiBlink")]
	public class MidiBlink : ReactiveZoneProgram
	{
		private InputDevice MidiInput { get; set; }
		
		public MidiBlink()
		{
			if (InputDevice.DeviceCount == 0)
			{
				throw new Exception("No MIDI devices found.");
			}

			try
			{
				MidiInput = new InputDevice(0);
				MidiInput.ChannelMessageReceived += (sender, args) =>
				{
					//ProgramCommon.Blink(new List<Tuple<Color, int>>
					//{
					//	{Color.FromArgb(args.Message.Data1 * 10, 50, 50), 500},
					//	{Color.Empty, 200}
					//}, OutputColor, SyncContext);

					if (args.Message.Data1 == 9)
					{
						OutputColor(Color.FromArgb(args.Message.Data2, 50, 50));
					}
					else if (args.Message.Data1 == 10)
					{
						OutputColor(Color.FromArgb(50, args.Message.Data2, 50));
					}
				};

				MidiInput.SysCommonMessageReceived += (sender, args) => {};
				MidiInput.SysExMessageReceived += (sender, args) => { };
				MidiInput.SysRealtimeMessageReceived += (sender, args) => { };
				MidiInput.Error += (sender, args) => { };

				MidiInput.StartRecording();
			}
			catch (Exception ex)
			{
				throw new Exception("Crazy shit happened.", ex);
			}
		}

		protected override void SetupInterruptingInputs()
		{
			AddInterruptingInput<Color>("On", parametersObject =>
			{
				dynamic parameters = parametersObject;
				Color color = parameters.Color;
				OutputColor(color);
			});

			AddInterruptingInput<Color>("Off", parametersObject =>
			{
				OutputColor(Color.Black);
			});
		}

		private void OutputColor(Color colorToSet)
		{
			SendColor(colorToSet);
		}


		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
			MidiInput.StopRecording();
			MidiInput.Reset();
			MidiInput.Close();
		}
	}
}