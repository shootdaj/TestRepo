using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ZoneLighting
{
	/// <summary>
	/// Tools for ease of debugging.
	/// </summary>
	public static class DebugTools
	{
		public class DebugEvent
		{
			public decimal EventOrderingNumber;
			public string MethodName;
			public DateTime EventTime;
			public string Message;
		}

		private static List<DebugEvent> _events;
		public static List<DebugEvent> Events
		{
			get
			{
				lock (EventsLock)
				{
					return _events;
				}
			}
		}

		public static object EventsLock = new object();
		public static string DateTimeFormatString;
		public static bool OutputActive = false;
		public static bool Active = true;
		public static bool TriggerOutput = false;
		public static bool StarredOnly = false;
		private static string OutputFile { get; } = @"C:\Temp\ZoneLightingDebug.txt";

		private static StreamWriter FileWriter { get; set; }

		static DebugTools()
		{
			Initialize();
		}

		public static void Initialize()
		{
			_events = new List<DebugEvent>();
			DateTimeFormatString = "hh:mm:ss.FFF";
#if DEBUG
			OutputActive = true;
#endif
		}

		/// <summary>
		/// Clears all events
		/// </summary>
		public static void ClearEvents()
		{
			Events.Clear();
		}

		/// <summary>
		/// Adds a Debug Event
		/// </summary>
		/// <param name="methodName">Name of the method where this event is being added</param>
		/// <param name="eventOrderingNumber">If the event is to be placed at a specific ordering number, it can be specified</param>
		/// <param name="eventTime">A different time for the event can be specifiied (different than the time when the event is logged)</param>
		/// <param name="message">A message to describe the event</param>
		public static DebugEvent AddEvent(string methodName, string message, decimal eventOrderingNumber, DateTime? eventTime = null)
		{
			if (Active)
			{
				var x = new DebugEvent()
				{
					EventOrderingNumber = eventOrderingNumber,
					MethodName = methodName,
					EventTime = eventTime ?? DateTime.Now,
					Message = message
				};
				Events.Add(x);
				return x;
			}
			return null;
		}

		/// <summary>
		/// Adds a Debug Event
		/// </summary>
		/// <param name="methodName">Name of the method where this event is being added</param>
		/// <param name="message">A message to describe the event</param>
		/// <param name="eventTime">A different time for the event can be specifiied (different than the time when the event is logged)</param>
		/// <returns>Returns the event that was just added</returns>
		public static DebugEvent AddEvent(string methodName, string message, DateTime? eventTime = null)
		{
			try
			{
				if (Active)
				{
					if (!(!TriggerOutput && methodName.Contains("Trigger.")) &&
						(!StarredOnly || methodName.Contains("*")))
					{
						var x = new DebugEvent()
						{
							EventOrderingNumber = Events.Count() + 1,
							MethodName = methodName,
							EventTime = eventTime ?? DateTime.Now,
							Message = message
						};
						Events.Add(x);
						return x;
					}
				}
			}
			catch (Exception ex)
			{
				throw new WarningException("Error while trying to add a Debug Event.");
			}

			return null;
		}

		public static void Print(this DebugEvent debugEvent)
		{
			if (OutputActive)
			{
				PrintOut(debugEvent.EventOrderingNumber + "[" + debugEvent.EventTime.ToString(DateTimeFormatString) + "]" + debugEvent.MethodName + ": " +
								debugEvent.Message);
			}
		}



		/// <summary>
		/// Prints out a summary of all the events
		/// </summary>
		/// <param name="timeSpans">Should timespans between events be printed?</param>
		/// <param name="printTotal">Should the total time of all events be printed?</param>
		/// <param name="clearEvents">Should the events be cleared after printing?</param>
		/// <param name="deactivate">Should DebugTools be deactivated after printing?</param>
		/// <param name="printLabel">Should the labels for the times be printed?</param>
		/// <param name="longDurationThreshold">What is the duration for a timespan between two events to be marked as "long".</param>
		public static void Print(bool timeSpans = true, bool printTotal = true, bool clearEvents = false, bool deactivate = false, bool printLabel = false, int longDurationThreshold = 250)
		{
			if (OutputActive)
			{
				if (Events.Any())
				{
					File.WriteAllText(OutputFile, string.Empty); //clear output file

					DebugEvent lastDebugEvent = null;
					foreach (var debugEvent in Events.OrderBy(x => x.EventOrderingNumber))
					{
						if (timeSpans && lastDebugEvent != null)
						{
							var totalMillisecondDifference = ((int)((debugEvent.EventTime - lastDebugEvent.EventTime).TotalMilliseconds));

							PrintOut("");
							PrintOut("   " + totalMillisecondDifference.ToString() +
										(printLabel ? " ms" : "") +
										(totalMillisecondDifference > longDurationThreshold
											 ? "                            !---------LONG--------!"
											 : ""));
							PrintOut("");
						}
						debugEvent.Print();
						lastDebugEvent = debugEvent;
					}

					if (printTotal)
					{
						PrintOut("");
						PrintOut("----------------------------------------------");
						PrintOut("");
						PrintOut("Total Time: " + (Events.Last().EventTime - Events.First().EventTime).TotalMilliseconds +
									(printLabel ? " ms" : ""));
						PrintOut("");
					}

					if (deactivate)
						Active = false;

					if (clearEvents)
						Events.Clear();
				}
			}
		}

		public static void PrintOut(string text, bool console = false, bool debug = true)
		{
			if (console)
				Console.WriteLine(text);
			if (debug)
				Debug.Print(text);
			if (OutputFile != null)
			{
				using (StreamWriter outfile = File.AppendText(OutputFile))
				{
					outfile.WriteLine(text);
				}
			}
		}
	}
}
