
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			var textSpace = new TextView(this);
			textSpace.Text = "      ";
			layout.AddView(textSpace);

			var buttonExit = new Button(this);
			buttonExit.Text = "X";
			buttonExit.SetBackgroundColor(Color.Red);
			buttonExit.SetWidth(40);
			buttonExit.Click += (sender, e) =>
			{
				generation++;
				windowManager.RemoveView(layout);
				StopSelf();
			};
			layout.AddView(buttonExit, new ViewGroup.LayoutParams(150, 150));

			/*
			var textSpace2 = new TextView(this);
			textSpace2.Text = "      ";
			layout.AddView(textSpace2);

			var textVersion = new TextView(this);
			textVersion.SetBackgroundColor(Color.DarkGray);
			textVersion.Text = $"  {ClickSettings.Instance.Version}  ";
			layout.AddView(textVersion);
			*/


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
				//ClickMeasure();
				while (g == generation)
				{
					//Click();
					//await Click2();
					Click3();

					await Task.Delay(200);
				}
			}
			catch (Exception ex)
			{
				Log.Debug(nameof(OverlayService), $"{ex}");
				ClickSettings.Log += DateTime.Now.FormatGerman() + "\n" + $"exception:\n{ex}\n\n";
			}
		}

		public void Click3()
		{
			Log.Debug(nameof(OverlayService), $"Click3");

			var x = ClickSettings.Instance.ClickX;
			var y = ClickSettings.Instance.ClickY;

			var process = Java.Lang.Runtime.GetRuntime().Exec("su");
			using (var stream = process.OutputStream)
			{
				using (var sw = new StreamWriter(stream))
				{
					var script = "";
					for (int i = 0; i < 20; i++)
					{
						script += $@"sendevent /dev/input/event10 1 330 1
sendevent /dev/input/event10 3 48 34
sendevent /dev/input/event10 3 57 0
sendevent /dev/input/event10 3 53 {x}
sendevent /dev/input/event10 3 54 {y}
sendevent /dev/input/event10 0 2 0
sendevent /dev/input/event10 0 0 0
sendevent /dev/input/event10 1 330 0
sendevent /dev/input/event10 0 2 0
sendevent /dev/input/event10 0 0 0
";

						script += $"usleep 10000\n";
					}
					script += ($"exit\n");
					Log.Debug(nameof(OverlayService), $"Click3:\n{script}\n\n");
					sw.Write(script);
					sw.Flush();
					process.WaitFor();
				}
			}
		}

		Instrumentation inst;

		public async Task Click2()
		{
			if (inst == null) inst = new Instrumentation();

			var x = ClickSettings.Instance.ClickX;
			var y = ClickSettings.Instance.ClickY;

			var eDown = MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), MotionEventActions.Down, x, y, 0);
			var eUp = MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), MotionEventActions.Up, x, y, 0);

			eDown.SetSource(InputSourceType.Touchscreen);
			eUp.SetSource(InputSourceType.Touchscreen);

			Java.Lang.Class[] paramTypes = new Java.Lang.Class[2];
			paramTypes[0] = Java.Lang.Class.FromType(typeof(InputEvent));
			paramTypes[1] = Java.Lang.Integer.Type;
			var inputManager = GetSystemService(Context.InputService).JavaCast<Android.Hardware.Input.InputManager>();
			var hiddenMethod = inputManager.Class.GetMethod("injectInputEvent", paramTypes);

			Java.Lang.Object[] p = new Java.Lang.Object[2];
			p[0] = eDown;
			p[1] = 0;

			hiddenMethod.Invoke(inputManager, p);

			await Task.Delay(5);

			p[0] = eUp;

			hiddenMethod.Invoke(inputManager, p);


			//inst.SendPointerSync(eDown);
			//await Task.Delay(5);
			//inst.SendPointerSync(MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), MotionEventActions.Up, x, y, 0));

			ClickSettings.Log += DateTime.Now.FormatGerman() + "\n" + $"SendPointerSync\n";
		}

		public long delayMilliseconds = 0;

		public void ClickMeasure()
		{
			Log.Debug(nameof(OverlayService), $"ClickMeasure");

			var x = ClickSettings.Instance.ClickX;
			var y = ClickSettings.Instance.ClickY;

			var process = Java.Lang.Runtime.GetRuntime().Exec("su");
			using (var stream = process.OutputStream)
			{
				using (var sw = new StreamWriter(stream))
				{
					for (int i = 0; i < 10; i++)
					{
						sw.Write($"/system/bin/input tap {x} {y}\n");
					}
					sw.Write($"exit\n");
					sw.Flush();

					var stopwatch = new Stopwatch();
					stopwatch.Start();
					process.WaitFor();
					stopwatch.Stop();
					delayMilliseconds = stopwatch.ElapsedMilliseconds / 10;
					Log.Debug(nameof(OverlayService), $"ClickMeasure: stopwatch.ElapsedMilliseconds = {stopwatch.ElapsedMilliseconds} (for 10 input commands), delayMilliseconds => {delayMilliseconds}");
					ClickSettings.Log += DateTime.Now.FormatGerman() + "\n" + $"ClickMeasure: stopwatch.ElapsedMilliseconds = {stopwatch.ElapsedMilliseconds} (for 10 input commands), delayMilliseconds => {delayMilliseconds}\n";
				}
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
					var script = "";
					for (int i = 0; i < 2; i++)
					{
						for (int k = 0; k < 10; k++)
						{
							script += ($"/system/bin/input tap {x} {y} &\n");
						}
						//sw.Write($"/system/bin/input tap {x} {y}\n");
						script += ($"usleep {delayMilliseconds * 1000}\n");
						//sw.Write($"sleep 0.005\n");
					}
					script += ($"exit\n");
					Log.Debug(nameof(OverlayService), $"Click:\n{script}\n\n");
					ClickSettings.Log += DateTime.Now.FormatGerman() + "\n" + $"Click:\n{script}\n\n";
					sw.Write(script);
					sw.Flush();
					process.WaitFor();
				}
			}
		}
	}
}
