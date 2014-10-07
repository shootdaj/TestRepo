using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting
{
	public interface IInitializable
	{
		void Initialize();
		bool Initialized { get; }
		void Uninitialize();
	}

	public interface IInitializableBool : IInitializable
	{
		new bool Initialize();
		new bool Uninitialize();
	}
}
