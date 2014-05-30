using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Countly.Countly.SharedInstance.init("http://cloud.count.ly", "82b4cbd21e8a1a11e071ae2e6b8c0c9c50ef0417");

			NSNotificationCenter.DefaultCenter.AddObserver(null, (notifiction) => {
				Console.WriteLine(notifiction.Name);
			});

			NSNotificationCenter.DefaultCenter.AddObserver("UINavigationControllerDidShowViewControllerNotification", (notifiction) => {
				var vc = notifiction.UserInfo["UINavigationControllerNextVisibleViewController"] as UIViewController;
				Countly.Countly.SharedInstance.PostEvent(new Countly.Countly.CountlyEvent {
					Key = "Page View",
					Count = 1,
					Segmentation = new Dictionary<string,string> {
						{ "VC Type",vc.GetType().Name },
						{ "Page Title",vc.Title },
					},
				});

				Console.WriteLine(vc.Title);
			});

			window = new UIWindow(UIScreen.MainScreen.Bounds);
			window.RootViewController = new UINavigationController(new MainViewController());
			window.MakeKeyAndVisible();

			return true;
		}

		public override void WillEnterForeground(UIApplication application)
		{
			Countly.Countly.SharedInstance.OnStart();
		}

		public override void DidEnterBackground(UIApplication application)
		{
			Countly.Countly.SharedInstance.OnStop();
		}
	}
}

