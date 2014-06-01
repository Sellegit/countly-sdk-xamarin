using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Countly;

namespace Sample.Android
{
	[Activity(Label = "Sample.Android", MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate {
				button.Text = string.Format("{0} clicks!", count++);
				Countly.Countly.SharedInstance.PostEvent(new Countly.Countly.CountlyEvent {
					Key = "Button Click",
					Count = 1
				});
			};
		}

		protected override void OnStart()
		{
			Countly.Countly.SharedInstance.init(this.ApplicationContext, "YOUR_SERVER", "YOUR_APP_KEY");
			Countly.Countly.SharedInstance.OnStart();

			base.OnStart();
		}

		protected override void OnStop()
		{
			Countly.Countly.SharedInstance.OnStop();

			base.OnStop();
		}
	}
}
