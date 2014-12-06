//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace ZoneLighting.ZoneProgramNS
//{
//	public abstract class ParameterizedZoneProgram : ZoneProgram, IParametrizedZoneProgram
//	{
//		/// <summary>
//		/// Holds extra information that is to be passed into the program.
//		/// </summary>
//		[DataMember]
//		public ZoneProgramParameter ProgramParameter { get; set; }

//		protected abstract void Start(ZoneProgramParameter zoneProgramParameter);

//		protected override void Start()
//		{
//		}

//		public abstract override void Stop(bool force);

//		public abstract IEnumerable<Type> AllowedParameterTypes { get; }

//		public override void StartBase()
//		{
//			throw new Exception("Call the parameterized StartBase(ZoneProgramParameter parameter) instead.");
//		}

//		public void StartBase(ZoneProgramParameter parameter)
//		{
//			if (AllowedParameterTypes.Contains(parameter.GetType()))
//			{
//				ProgramParameter = parameter;
//				Start(parameter);
//			}
//			else
//			{
//				throw new Exception("Input parameter type is not an allowed parameter type for this zone program.");
//			}
//		}
//	}
//}
