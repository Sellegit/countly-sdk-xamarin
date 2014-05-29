using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Countly.Countly.SharedInstance.init ("http://countly.gmusicapp.com/", "dd6d7130db7f0f0143e7a44d202c8e7d2fd49125");
			NSNotificationCenter.DefaultCenter.AddObserver (null, (notifiction) => {
				Console.WriteLine(notifiction.Name);
			});
			NSNotificationCenter.DefaultCenter.AddObserver ("UINavigationControllerDidShowViewControllerNotification", (notifiction) => {
				var vc = notifiction.UserInfo["UINavigationControllerNextVisibleViewController"] as UIViewController;
				Countly.Countly.SharedInstance.PostEvent(new Countly.Countly.CountlyEvent{
					Key = "Page View",
					Count = 1,
					Segmentation = new Dictionary<string,string>{
						{"VC Type",vc.GetType().Name},
						{"Page Title",vc.Title},
					},
				});
				Console.WriteLine(vc.Title);
			});
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;
			window.RootViewController = new UINavigationController( new MainViewController ());
			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}
		public override void WillEnterForeground (UIApplication application)
		{
			Countly.Countly.SharedInstance.OnStart ();
		}
		public override void DidEnterBackground (UIApplication application)
		{
			Countly.Countly.SharedInstance.OnStop ();
		}
	}
}

