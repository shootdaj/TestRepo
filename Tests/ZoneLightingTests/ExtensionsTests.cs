using NUnit.Framework;
using ZoneLighting;

namespace ZoneLightingTests
{
	public class ExtensionsTests
	{
		[Test]
		public void Listify_ArrayOfObjects_ReturnsList()
		{
			Assert.That(1.Listify(), Contains.Item(1));
		}
	}
}
