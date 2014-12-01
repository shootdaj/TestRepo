using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.Config
{
	public class Config
	{
		public static void SaveZone(IEnumerable<Zone> zones, string filename)
		{
			//XmlSerializer xsSubmit = new XmlSerializer(typeof(Zone));
			//StringWriter sww = new StringWriter();
			//XmlWriter writer = XmlWriter.Create(sww);
			////zones.ToList().ForEach(x => xsSubmit.Serialize(writer, x));
			//xsSubmit.Serialize(writer, zones);
			//var xml = sww.ToString(); // Your xml

			//using (var writer = new StreamWriter(filename))
			//{
			//	var serializer = new XmlSerializer(typeof(Zone));
			//	serializer.Serialize(writer, zones);
			//	writer.Flush();
			//}

			//zones.ToList().ForEach(zone => );

			//XDocument doc = new XDocument();

			//zones.ToList().ForEach(zone => doc.Add(new XElement(zone.Name, 
			//	new XElement(zone.ZoneProgram.GetType().ToString(),
			//		new XElement(zone.ZoneProgram.ProgramParameter)))));


			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented
			};

			string json = JsonConvert.SerializeObject(zones, settings);
			File.WriteAllText(filename, json);
		}

		


		///// <summary>
		///// Tries to find and return the value of the first key found with the given name
		///// </summary>
		///// <param name="xmlDoc">XML Document to search</param>
		///// <param name="key">Key to search for</param>
		///// <param name="multiplicity">Number of total results</param>
		///// <param name="entryTagName">Entry tag name to search for</param>
		///// <param name="keyAttribute">Key attribute to search for</param>
		///// <param name="valueAttribute">Value attribute to search for</param>
		///// <returns>Value of the first key found, if any</returns>
		//public static string GetConfigValueFromAnywhere(string key, XmlDocument xmlDoc, out int multiplicity,
		//	string entryTagName = "add", string keyAttribute = "key", string valueAttribute = "value")
		//{
		//	List<string> values = new List<string>();
		//	xmlDoc.IterateThroughAllNodes((xmlNode) =>
		//	{
		//		if (xmlNode.Attributes != null && xmlNode.Attributes["key"] != null && xmlNode.Attributes["key"].Value == key)
		//		{
		//			values.Add(xmlNode.Attributes["value"].Value);
		//		}
		//	});

		//	multiplicity = values.Count;
		//	return multiplicity > 0 ? values[0] : "";
		//}
	}
}
