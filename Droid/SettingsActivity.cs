
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ClickTest.Droid
{
	[Activity(Label = "SettingsActivity", Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SettingsActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public static SettingsActivity Instance = null;
		protected override void OnCreate(Bundle bundle)
		{
			Instance = this;
			base.OnCreate(bundle);

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			try
			{
				var process = Java.Lang.Runtime.GetRuntime().Exec("su");
				using (var stream = process.OutputStream)
				{
					using (var sw = new System.IO.StreamWriter(stream))
					{
						sw.Write($"exit\n");
						sw.Flush();
					}
				}
			}
			catch (Exception) { }

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
		}

	}
}
