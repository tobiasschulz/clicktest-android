using System;
namespace ClickTest.Droid
{
	public class ClickSettings
	{
		public static ClickSettings Instance = new ClickSettings();

		public int ClickX { get; set; } = 540;
		public int ClickY { get; set; } = 960;
	}
}
