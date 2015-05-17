using System.Drawing;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class InputStartingValuesTests
	{
		[Test]
		public void InputStartingValues_SetParameter_CreatesNewDictionaryEntry()
		{
			dynamic isv = new ISV();
			isv.Speed = 1;
			isv.Color = Color.Red;
			isv.Name = "Test";

			Assert.True(isv["Speed"] == 1);
			Assert.True(isv["Color"] == Color.Red);
			Assert.True(isv["Name"] == "Test");
		}

		[Test]
		public void InputStartingValues_AddParameter_IsAccessibleUsingDotNotation()
		{
			dynamic isv = new ISV();
			isv.Add("Speed", 1);
			isv.Add("Color", Color.Red);
			isv.Add("Name", "Test");

			Assert.True(isv.Speed == 1);
			Assert.True(isv.Color == Color.Red);
			Assert.True(isv.Name == "Test");
		}
	}
}
