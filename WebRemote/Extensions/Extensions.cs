using System.Linq;
using WebRemote.IoC;
using WebRemote.Models;
using ZoneLighting;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.Extensions
{
	public static class Extensions
	{
		
		public static TDestination ToJsonModel<TSource, TDestination>(this TSource input)
		{
			var output = Container.AutoMapper.Map<TSource, TDestination>(input);
			return output;
		}

		public static ZLMJsonModel ToZLMJsonModel(this ZLM zlm)
		{
			var output = Container.AutoMapper.Map<ZLM, ZLMJsonModel>(zlm);
			//output.ProgramSets = zlm.ProgramSets.Select(x => x.ToJsonModel<ProgramSet, ProgramSetJsonModel>()).ToBetterList();
			//output.Zones = zlm.Zones.Select(x => x.ToZoneJsonModel()).ToBetterList();

			return output;
		}

		public static ZoneJsonModel ToZoneJsonModel(this Zone zone)
		{
			var output = zone.ToJsonModel<Zone, ZoneJsonModel>();
			output.Inputs = zone.ZoneProgram.Inputs.ToISV();

			return output;
		}
	}
}