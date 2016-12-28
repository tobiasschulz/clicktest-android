using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ClickTest.Droid;
using Android.Provider;

[assembly: UsesPermission(Name = "android.permission.SYSTEM_ALERT_WINDOW")]
[assembly: UsesPermission(Name = "android.permission.INJECT_EVENTS")]

namespace ClickTest
{
	[Activity(Label = "ClickTest", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
		public static MainActivity Instance = null;
		public static int ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE = 5469;

		protected override void OnCreate(Bundle bundle)
		{
			Instance = this;

			base.OnCreate(bundle);

			checkPermission();
		}

		protected override void OnResume()
		{
			base.OnResume();

			checkPermission();
		}

		protected void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (requestCode == ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE)
			{
				if (!Settings.CanDrawOverlays(this))
				{
					// You don't have permission
					checkPermission();
				}
				else
				{
					//do as per your logic 
					StartActivity(typeof(SettingsActivity));
				}

			}
		}

		public void checkPermission()
		{
			if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
			{
				if (!Settings.CanDrawOverlays(this))
				{
					Intent intent = new Intent(Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + PackageName));
					StartActivityForResult(intent, ACTION_MANAGE_OVERLAY_PERMISSION_REQUEST_CODE);
					return;
				}
			}
			StartActivity(typeof(SettingsActivity));
		}
	}
}
