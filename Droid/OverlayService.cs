﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ClickTest.Droid
{
	[Service(Label = "OverlayService")]
	public class OverlayService : Service
	{
		public override void OnCreate()
		{
			base.OnCreate();

			Log.Debug(nameof(OverlayService), $"OnCreate");
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug(nameof(OverlayService), $"OnStartCommand");
			OpenOverlay();

			return StartCommandResult.Sticky;
		}

		bool isEnabled = false;
		int generation = 0;

		void OpenOverlay()
		{
			var windowManager = GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

			var layout = new LinearLayout(this);
			var button = new Button(this);
			button.Text = "Start";
			button.Click += (sender, e) =>
			{
				isEnabled = !isEnabled;
				button.Text = isEnabled ? "Stop" : "Start";
				if (isEnabled)
				{
					Task.Run(async () => await Loop()).Forget();
				}
				else {
					generation++;
				}
			};
			layout.AddView(button);

			var _params = new WindowManagerLayoutParams(
				ViewGroup.LayoutParams.WrapContent,
				ViewGroup.LayoutParams.WrapContent,
				WindowManagerTypes.Phone,
				WindowManagerFlags.NotTouchModal,
				Format.Translucent
			);

			_params.Gravity = GravityFlags.Top | GravityFlags.Left;
			_params.X = 100;
			_params.Y = 100;

			windowManager.AddView(layout, _params);
		}

		public override IBinder OnBind(Intent intent)
		{
			// This is a started service, not a bound service, so we just return null.
			return null;
		}

		public async Task Loop()
		{
			try
			{
				Log.Debug(nameof(OverlayService), $"Loop");

				int g = generation;
				while (g == generation)
				{
					Click();

					await Task.Delay(10);
				}
			}
			catch (Exception ex)
			{
				Log.Debug(nameof(OverlayService), $"{ex}");
			}
		}

		public void Click()
		{
			Log.Debug(nameof(OverlayService), $"Click");

			var x = ClickSettings.Instance.ClickX;
			var y = ClickSettings.Instance.ClickY;

			var process = Java.Lang.Runtime.GetRuntime().Exec("su");
			using (var stream = process.OutputStream)
			{
				using (var sw = new StreamWriter(stream))
				{
					for (int i = 0; i < 100; i++)
					{
						sw.Write($"/system/bin/input tap {x} {y}\n");
						sw.Write($"sleep 0.005\n");
					}
					sw.Write($"exit\n");
					sw.Flush();
					process.WaitFor();
				}
			}
		}
	}
}
