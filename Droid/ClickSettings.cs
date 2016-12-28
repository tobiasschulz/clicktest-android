using System;
namespace ClickTest.Droid
{
	public class ClickSettings
	{
		public static ClickSettings Instance = new ClickSettings();

		public int Version { get; set; } = 13;

		public int ClickX { get; set; } = 540;
		public int ClickY { get; set; } = 960;

		public static string Log { get; set; } = string.Empty;
	}
}
