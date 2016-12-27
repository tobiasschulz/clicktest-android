using System;

using Xamarin.Forms;

namespace ClickTest
{
	public class App : Application
	{
		public App()
		{
			var content = new SettingsPage();
			MainPage = new NavigationPage(content);
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
