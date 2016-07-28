using AutoMapper;
using WebRemote.Models;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.Automapper
{
	//public class ZoneJsonModelConverter : ITypeConverter<Zone, ZoneJsonModel>
	//{
	//	public ZoneJsonModel Convert(Zone source, ZoneJsonModel destination, ResolutionContext context)
	//	{
	//		destination = context.Mapper.Map<Zone, ZoneJsonModel>(source);
	//		destination.Inputs = source.ZoneProgram.Inputs.ToISV().Dictionary;

	//		return destination;
	//	}
	//}

	public class ZoneJsonModelInputsResolver : IValueResolver<Zone, ZoneJsonModel, ISV>
	{
		public ISV Resolve(Zone source, ZoneJsonModel destination, ISV destMember, ResolutionContext context)
		{
			return source.ZoneProgram.Inputs.ToISV();
		}
	}
}
