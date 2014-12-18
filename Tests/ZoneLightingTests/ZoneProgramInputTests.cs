using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Xunit;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneProgramInputTests
	{
		[Fact]
		public void ZoneProgramInputCollection_CannotAddNullTypes()
		{
			Assert.Throws<Exception>(() => new ZoneProgramInputCollection().Add(null));
		}
	}
}
