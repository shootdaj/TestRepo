using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class InputStartingValuesTests
	{
		[Test]
		public void InputStartingValues_SetParameter_CreatesNewDictionaryEntry()
		{
			dynamic isv = new InputStartingValues();
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
			dynamic isv = new InputStartingValues();
			isv.Add("Speed", 1);
			isv.Add("Color", Color.Red);
			isv.Add("Name", "Test");

			Assert.True(isv.Speed == 1);
			Assert.True(isv.Color == Color.Red);
			Assert.True(isv.Name == "Test");
		}
	}
}
