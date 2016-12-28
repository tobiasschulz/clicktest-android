using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using ClickTest.Droid;
using Xamarin.Forms;

namespace ClickTest
{
	public class SettingsPage : ContentPage
	{
		Editor editorLog;
		public SettingsPage()
		{
			Title = "ClickTest " + ClickSettings.Instance.Version;

			var entryX = new Entry
			{
				Text = ClickSettings.Instance.ClickX.ToString(),
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			entryX.TextChanged += (sender, e) => ClickSettings.Instance.ClickX = e.NewTextValue.ToInteger();

			var entryY = new Entry
			{
				Text = ClickSettings.Instance.ClickY.ToString(),
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			entryY.TextChanged += (sender, e) => ClickSettings.Instance.ClickY = e.NewTextValue.ToInteger();

			editorLog = new Editor
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Text = ClickSettings.Log,
				HeightRequest = 500,
			};
			Task.Run(async () => await refreshLog()).Forget();

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children = {
					new Xamarin.Forms.Button {
						Text = "Open Overlay",
						Command = new Command((obj) => showOverlay()),
					},
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						Children = {
							new Label {
								HorizontalTextAlignment = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Text = "X:",
							},
							entryX,
						},
					},
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						Children = {
							new Label {
								HorizontalTextAlignment = TextAlignment.Center,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Text = "Y:",
							},
							entryY,
						},
					},
					new Xamarin.Forms.Button {
						Text = "Copy",
						Command = new Command(()=>{
							try {
								Intent sendIntent = new Intent();
								sendIntent.SetAction(Intent.ActionSend);
								sendIntent.PutExtra(Intent.ExtraText, ClickSettings.Log);
								sendIntent.AddFlags(ActivityFlags.NewTask);
								sendIntent.SetType("text/plain");
								SettingsActivity.Instance.StartActivity(sendIntent);

							} catch (Exception ex) {
								ClickSettings.Log += $"\n{ex}\n";
							}
						}),
					},
					editorLog,
				},
			};
		}

		private void showOverlay()
		{
			MainActivity.Instance.StartService(new Intent(MainActivity.Instance, typeof(OverlayService)));
		}

		private async Task refreshLog()
		{
			while (true)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					editorLog.Text = ClickSettings.Log;
				});

				await Task.Delay(10 * 1000);
			}
		}
	}
}
