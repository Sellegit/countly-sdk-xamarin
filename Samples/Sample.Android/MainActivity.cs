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
			};
		}

		protected override void OnStart()
		{
			base.OnStart();

			Countly.Countly.SharedInstance.init(this.ApplicationContext, "http://cloud.count.ly", "82b4cbd21e8a1a11e071ae2e6b8c0c9c50ef0417");
			Countly.Countly.SharedInstance.OnStart();
		}

		protected override void OnStop()
		{
			base.OnStop();

			Countly.Countly.SharedInstance.OnStop();
		}
	}
}


