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
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			Countly.Countly.SharedInstance.init ("https://cloud.count.ly", "b63753625c3ff1399e9a1e797b96bec2b872715c");
			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;
			window.RootViewController = new MainViewController ();
			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
		public override void WillEnterForeground (UIApplication application)
		{
			Countly.Countly.SharedInstance.OnStart ();
		}
	}
}

