using System;
using Android.App;
using Android.Content;
using Android.Widget;
using ClickTest.Droid;
using Xamarin.Forms;

namespace ClickTest
{
	public class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			Title = "ClickTest";

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

			var editorLog = new Editor
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Text = ClickSettings.Log,
			};

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
					editorLog,
				},
			};
		}

		private void showOverlay()
		{
			MainActivity.Instance.StartService(new Intent(MainActivity.Instance, typeof(OverlayService)));
		}
	}
}
